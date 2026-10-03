using CRM.Core.Common.Abstraction;
using CRM.Core.Common.Configuration;
using CRM.Core.Medias.Abstractions;
using Microsoft.Extensions.Options;

namespace CRM.Infrastructure.Medias.Storage;

// Adapted from TenantExchange2026's local storage: safe keys and atomic file writes.
public sealed class LocalFileMediaStorage : IMediaStorage
{
    private readonly String _strRoot;

    public LocalFileMediaStorage(IOptions<CRMConfiguration> objOptions, IAppPathProvider objPaths)
    {
        String strRoot = objOptions.Value.MediaSettings.Root;
        if (String.IsNullOrWhiteSpace(strRoot))
        {
            throw new InvalidOperationException("CRM:MediaSettings:Root is required.");
        }

        _strRoot = Path.GetFullPath(strRoot, objPaths.ContentRootPath);
        String strWebRoot = Path.GetFullPath(objPaths.WebRootPath);
        if (IsWithin(_strRoot, strWebRoot))
        {
            throw new InvalidOperationException("Media must be stored outside the public web root.");
        }
    }

    public async Task<Int64> SaveAsync(String strKey, Stream stmContent, Int64 lngMaxBytes, CancellationToken objToken = default)
    {
        if (lngMaxBytes <= 0)
        {
            throw new InvalidOperationException("The media upload size limit must be greater than zero.");
        }

        String strPath = SafeCombine(strKey);
        Directory.CreateDirectory(Path.GetDirectoryName(strPath)!);
        String strTemporaryPath = strPath + ".tmp-" + Guid.NewGuid().ToString("N");

        try
        {
            Int64 lngSize = 0;
            await using (FileStream stmFile = new(strTemporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            {
                Byte[] colBuffer = new Byte[81920];
                Int32 intRead;
                while ((intRead = await stmContent.ReadAsync(colBuffer, objToken)) > 0)
                {
                    lngSize += intRead;
                    if (lngSize > lngMaxBytes)
                    {
                        throw new InvalidDataException($"Each file must be no larger than {lngMaxBytes / (1024 * 1024)} MB.");
                    }

                    await stmFile.WriteAsync(colBuffer.AsMemory(0, intRead), objToken);
                }

                if (lngSize == 0)
                {
                    throw new InvalidDataException("Empty files cannot be attached.");
                }
            }

            objToken.ThrowIfCancellationRequested();
            File.Move(strTemporaryPath, strPath, false);
            return lngSize;
        }
        finally
        {
            File.Delete(strTemporaryPath);
        }
    }

    public Task<Stream> OpenReadAsync(String strKey, CancellationToken objToken = default)
    {
        objToken.ThrowIfCancellationRequested();
        return Task.FromResult<Stream>(new FileStream(
            SafeCombine(strKey), FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 81920, true));
    }

    public Task DeleteAsync(String strKey, CancellationToken objToken = default)
    {
        objToken.ThrowIfCancellationRequested();
        File.Delete(SafeCombine(strKey));
        return Task.CompletedTask;
    }

    private String SafeCombine(String strKey)
    {
        if (String.IsNullOrWhiteSpace(strKey) || Path.IsPathRooted(strKey) || strKey.Contains(':'))
        {
            throw new UnauthorizedAccessException("Invalid media storage key.");
        }

        String strPath = Path.GetFullPath(Path.Combine(_strRoot, strKey.Replace('/', Path.DirectorySeparatorChar)));
        if (!IsWithin(strPath, _strRoot) || String.Equals(strPath, _strRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("The media key must remain within the storage directory.");
        }

        return strPath;
    }

    private static Boolean IsWithin(String strPath, String strRoot) =>
        String.Equals(strPath, strRoot, StringComparison.OrdinalIgnoreCase) ||
        strPath.StartsWith(Path.TrimEndingDirectorySeparator(strRoot) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}
