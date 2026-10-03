using CRM.Core.Medias.Abstractions;
using CRM.Core.Medias.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Medias.Repositories;

public sealed class EfMediaRepository(IDbContextFactory<CRMDbContext> objFactory) : IMediaRepository
{
    public async Task<Media?> GetAsync(Guid objMediaId, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        return await objContext.Medias.AsNoTracking()
            .FirstOrDefaultAsync(objMedia => objMedia.Id == objMediaId && !objMedia.DeletedUtc.HasValue, objToken);
    }

    public async Task<List<Media>> GetByJobIdAsync(Guid objJobId, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        return await objContext.Medias.AsNoTracking()
            .Where(objMedia => objMedia.JobId == objJobId && !objMedia.DeletedUtc.HasValue)
            .OrderByDescending(objMedia => objMedia.CreatedUtc)
            .ThenBy(objMedia => objMedia.Id)
            .ToListAsync(objToken);
    }

    public async Task AddAsync(Media objMedia, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        objContext.Medias.Add(objMedia);
        await objContext.SaveChangesAsync(objToken);
    }

    public async Task MarkDeletedAsync(Guid objMediaId, Guid objUserId, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        await objContext.Medias
            .Where(objMedia => objMedia.Id == objMediaId && !objMedia.DeletedUtc.HasValue)
            .ExecuteUpdateAsync(objSetters => objSetters
                .SetProperty(objMedia => objMedia.DeletedUtc, DateTime.UtcNow)
                .SetProperty(objMedia => objMedia.DeletedByUserId, objUserId), objToken);
    }
}
