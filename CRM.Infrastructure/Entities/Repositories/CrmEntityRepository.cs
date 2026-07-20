using CRM.Core.Entities.Abstractions;
using CRM.Core.Entities.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Entities.Repositories
{
    public sealed class CrmEntityRepository : ICrmEntityRepository
    {
        private readonly CRMDbContext _context;

        public CrmEntityRepository(CRMDbContext objContext)
        {
            _context = objContext;
        }

        public async Task<CrmEntity?> GetEntityByIdAsync(Guid objEntityId, Boolean blnAsTracking = false, CancellationToken objToken = default)
        {
            IQueryable<CrmEntity> colQuery = _context.Entities
                .Include(x => x.EntityType);

            if (!blnAsTracking)
            {
                colQuery = colQuery.AsNoTracking();
            }

            return await colQuery.FirstOrDefaultAsync(x => x.Id == objEntityId, objToken);
        }

        public async Task<List<CrmEntity>> GetEntitiesAsync(
            Int32? intEntityTypeId = null,
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<CrmEntity> colQuery = _context.Entities
                .Include(x => x.EntityType)
                .AsNoTracking();

            if (intEntityTypeId.HasValue)
            {
                colQuery = colQuery.Where(x => x.EntityTypeId == intEntityTypeId.Value);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strSearchTerm = strSearch.Trim();

                colQuery = colQuery.Where(x => x.DisplayName.Contains(strSearchTerm));
            }

            if (!blnIncludeArchived)
            {
                colQuery = colQuery.Where(x => !x.ArchivedUtc.HasValue);
            }

            if (!blnIncludeDeleted)
            {
                colQuery = colQuery.Where(x => !x.DeletedUtc.HasValue);
            }

            return await colQuery
                .OrderBy(x => x.DisplayName)
                .ToListAsync(objToken);
        }

        public async Task<Boolean> EntityExistsAsync(Guid objEntityId, CancellationToken objToken = default)
        {
            return await _context.Entities
                .AnyAsync(x => x.Id == objEntityId && !x.DeletedUtc.HasValue, objToken);
        }

        public async Task AddEntityAsync(CrmEntity objEntity, CancellationToken objToken = default)
        {
            await _context.Entities.AddAsync(objEntity, objToken);
        }

        public async Task SaveChangesAsync(CancellationToken objToken = default)
        {
            await _context.SaveChangesAsync(objToken);
        }
    }
}