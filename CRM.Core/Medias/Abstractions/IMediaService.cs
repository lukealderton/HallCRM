using CRM.Core.Medias.Domain;

namespace CRM.Core.Medias.Abstractions;

public interface IMediaService
{
    Task<List<Media>> GetByJobIdAsync(Guid objJobId, Guid objRequesterId, CancellationToken objToken = default);
    Task<Media> SaveMediaAsync(Guid objJobId, Guid objRequesterId, String strFileName, Stream stmContent, CancellationToken objToken = default);
    Task<MediaReadResult?> OpenForReadAsync(Guid objMediaId, Guid objRequesterId, CancellationToken objToken = default);
    Task DeleteAsync(Guid objMediaId, Guid objRequesterId, CancellationToken objToken = default);
}

public sealed record MediaReadResult(Media Media, Stream Content);
