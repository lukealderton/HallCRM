using CRM.Core.Medias.Domain;

namespace CRM.Core.Medias.Abstractions;

public interface IMediaRepository
{
    Task<Media?> GetAsync(Guid objMediaId, CancellationToken objToken = default);
    Task<List<Media>> GetByJobIdAsync(Guid objJobId, CancellationToken objToken = default);
    Task AddAsync(Media objMedia, CancellationToken objToken = default);
    Task MarkDeletedAsync(Guid objMediaId, Guid objUserId, CancellationToken objToken = default);
}
