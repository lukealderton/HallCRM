using CRM.Core.Common.Configuration;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Core.Medias.Abstractions;
using CRM.Core.Medias.Domain;
using CRM.Core.Users.Abstraction.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Core.Medias.Services;

// Adapted from TenantExchange2026: metadata repository plus private original-file storage.
public sealed class MediaService(
    IMediaRepository objRepository,
    IMediaStorage objStorage,
    IJobService objJobService,
    IUserService objUserService,
    IOptions<CRMConfiguration> objOptions,
    ILogger<MediaService> objLogger) : IMediaService
{
    public async Task<List<Media>> GetByJobIdAsync(Guid objJobId, Guid objRequesterId, CancellationToken objToken = default)
    {
        await EnsureAccessAsync(objJobId, objRequesterId, false, objToken);
        return await objRepository.GetByJobIdAsync(objJobId, objToken);
    }

    public async Task<Media> SaveMediaAsync(Guid objJobId, Guid objRequesterId, String strFileName, Stream stmContent, CancellationToken objToken = default)
    {
        await EnsureAccessAsync(objJobId, objRequesterId, true, objToken);
        String strName = Path.GetFileName(strFileName.Replace('\\', '/')).Trim();
        if (String.IsNullOrWhiteSpace(strName) || strName.Length > 255 || strName.Any(Char.IsControl))
        {
            throw new InvalidDataException("Choose a file with a valid name of up to 255 characters.");
        }

        String strExtension = Path.GetExtension(strName).ToLowerInvariant();
        String strContentType = GetContentType(strExtension);
        Media objMedia = new()
        {
            Id = Guid.NewGuid(),
            JobId = objJobId,
            OwnerId = objRequesterId,
            FileName = strName,
            ContentType = strContentType,
            CreatedUtc = DateTime.UtcNow
        };
        objMedia.Key = $"jobs/{objJobId:N}/{objMedia.Id:N}/original{strExtension}";

        try
        {
            objMedia.SizeBytes = await objStorage.SaveAsync(
                objMedia.Key, stmContent, objOptions.Value.MediaSettings.MaxFileSizeBytes, objToken);

            await using (Stream stmSaved = await objStorage.OpenReadAsync(objMedia.Key, objToken))
            {
                Byte[] colHeader = new Byte[32];
                Int32 intRead = await stmSaved.ReadAtLeastAsync(colHeader, colHeader.Length, false, objToken);
                if (!HasValidHeader(strExtension, colHeader.AsSpan(0, intRead)))
                {
                    throw new InvalidDataException("The file contents do not match a supported media format.");
                }
            }

            await objRepository.AddAsync(objMedia, objToken);
            return objMedia;
        }
        catch
        {
            await TryDeleteFileAsync(objMedia.Key);
            throw;
        }
    }

    public async Task<MediaReadResult?> OpenForReadAsync(Guid objMediaId, Guid objRequesterId, CancellationToken objToken = default)
    {
        Media? objMedia = await objRepository.GetAsync(objMediaId, objToken);
        if (objMedia == null)
        {
            return null;
        }

        await EnsureAccessAsync(objMedia.JobId, objRequesterId, false, objToken);
        try
        {
            return new(objMedia, await objStorage.OpenReadAsync(objMedia.Key, objToken));
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
    }

    public async Task DeleteAsync(Guid objMediaId, Guid objRequesterId, CancellationToken objToken = default)
    {
        Media? objMedia = await objRepository.GetAsync(objMediaId, objToken);
        if (objMedia == null)
        {
            return;
        }

        await EnsureAccessAsync(objMedia.JobId, objRequesterId, true, objToken);
        await objRepository.MarkDeletedAsync(objMediaId, objRequesterId, objToken);
        await TryDeleteFileAsync(objMedia.Key);
    }

    private async Task EnsureAccessAsync(Guid objJobId, Guid objRequesterId, Boolean blnModify, CancellationToken objToken)
    {
        if (objRequesterId == Guid.Empty || (await objUserService.GetUserAsync(objRequesterId, objToken))?.Enabled != true)
        {
            throw new UnauthorizedAccessException("You must be signed in as an enabled user to access job media.");
        }

        Job? objJob = await objJobService.GetJobByIdAsync(objJobId, objToken);
        if (objJob == null || objJob.IsDeleted)
        {
            throw new KeyNotFoundException("Job not found.");
        }

        // CRM jobs are shared with enabled staff; archived jobs remain readable.
        if (blnModify && objJob.IsArchived)
        {
            throw new InvalidOperationException("Restore the job before changing its media.");
        }
    }

    private async Task TryDeleteFileAsync(String strKey)
    {
        try
        {
            await objStorage.DeleteAsync(strKey, CancellationToken.None);
        }
        catch (Exception objError)
        {
            objLogger.LogError(objError, "Failed to remove media file {MediaKey}", strKey);
        }
    }

    private static String GetContentType(String strExtension) => strExtension switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        ".heic" => "image/heic",
        ".heif" => "image/heif",
        ".mp4" => "video/mp4",
        ".mov" => "video/quicktime",
        ".webm" => "video/webm",
        ".pdf" => "application/pdf",
        _ => throw new InvalidDataException("Supported files: JPG, PNG, GIF, WebP, HEIC, HEIF, MP4, MOV, WebM and PDF.")
    };

    private static Boolean HasValidHeader(String strExtension, ReadOnlySpan<Byte> colHeader)
    {
        return strExtension switch
        {
            ".jpg" or ".jpeg" => colHeader.StartsWith<Byte>([0xff, 0xd8, 0xff]),
            ".png" => colHeader.StartsWith<Byte>([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a]),
            ".gif" => colHeader.StartsWith("GIF87a"u8) || colHeader.StartsWith("GIF89a"u8),
            ".webp" => colHeader.Length >= 12 && colHeader[..4].SequenceEqual("RIFF"u8) && colHeader[8..12].SequenceEqual("WEBP"u8),
            ".pdf" => colHeader.StartsWith("%PDF-"u8),
            ".webm" => colHeader.StartsWith<Byte>([0x1a, 0x45, 0xdf, 0xa3]),
            ".heic" or ".heif" => colHeader.Length >= 12 && colHeader[4..8].SequenceEqual("ftyp"u8) &&
                (colHeader[8..12].SequenceEqual("heic"u8) || colHeader[8..12].SequenceEqual("heix"u8) ||
                 colHeader[8..12].SequenceEqual("heif"u8) || colHeader[8..12].SequenceEqual("mif1"u8)),
            ".mp4" => colHeader.Length >= 12 && colHeader[4..8].SequenceEqual("ftyp"u8) &&
                (colHeader[8..12].SequenceEqual("isom"u8) || colHeader[8..12].SequenceEqual("iso2"u8) ||
                 colHeader[8..12].SequenceEqual("mp41"u8) || colHeader[8..12].SequenceEqual("mp42"u8) ||
                 colHeader[8..12].SequenceEqual("avc1"u8) || colHeader[8..12].SequenceEqual("M4V "u8)),
            ".mov" => colHeader.Length >= 12 && colHeader[4..8].SequenceEqual("ftyp"u8) && colHeader[8..12].SequenceEqual("qt  "u8),
            _ => false
        };
    }
}
