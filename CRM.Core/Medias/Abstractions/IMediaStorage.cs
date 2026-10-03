namespace CRM.Core.Medias.Abstractions;

public interface IMediaStorage
{
    Task<Int64> SaveAsync(String strKey, Stream stmContent, Int64 lngMaxBytes, CancellationToken objToken = default);
    Task<Stream> OpenReadAsync(String strKey, CancellationToken objToken = default);
    Task DeleteAsync(String strKey, CancellationToken objToken = default);
}
