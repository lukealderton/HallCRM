using CRM.Core.Entities.Abstractions;
using CRM.Core.Entities.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Entities.Repositories
{
    public sealed class CrmEntityRepository : ICrmEntityRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public CrmEntityRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory = objDbContextFactory;
        }

        ///<inheritdoc/>
        public async Task<CrmEntity?> GetEntityByIdAsync(
            Guid objEntityId,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Entities
                .AsNoTracking()
                .Include(objEntity => objEntity.EntityType)
                .FirstOrDefaultAsync(
                    objEntity => objEntity.Id == objEntityId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<CrmEntity>> GetEntitiesAsync(
            Int32? intEntityTypeId = null,
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<CrmEntity> colQuery =
                objDbContext.Entities
                    .AsNoTracking()
                    .Include(objEntity => objEntity.EntityType);

            if (intEntityTypeId.HasValue)
            {
                colQuery = colQuery.Where(objEntity =>
                    objEntity.EntityTypeId == intEntityTypeId.Value);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strSearchTerm =
                    strSearch.Trim();

                colQuery = colQuery.Where(objEntity =>
                    objEntity.DisplayName.Contains(strSearchTerm));
            }

            if (!blnIncludeArchived)
            {
                colQuery = colQuery.Where(objEntity =>
                    !objEntity.ArchivedUtc.HasValue);
            }

            if (!blnIncludeDeleted)
            {
                colQuery = colQuery.Where(objEntity =>
                    !objEntity.DeletedUtc.HasValue);
            }

            return await colQuery
                .OrderBy(objEntity => objEntity.DisplayName)
                .ToListAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> EntityExistsAsync(
            Guid objEntityId,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Entities
                .AsNoTracking()
                .AnyAsync(
                    objEntity =>
                        objEntity.Id == objEntityId &&
                        !objEntity.DeletedUtc.HasValue,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task AddEntityAsync(
            CrmEntity objEntity,
            CancellationToken objToken = default)
        {
            if (objEntity == null)
            {
                throw new ArgumentNullException(nameof(objEntity));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            await objDbContext.Entities.AddAsync(
                objEntity,
                objToken);

            await objDbContext.SaveChangesAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task UpdateEntityAsync(
            CrmEntity objEntity,
            CancellationToken objToken = default)
        {
            if (objEntity == null)
            {
                throw new ArgumentNullException(nameof(objEntity));
            }

            if (objEntity.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Entity id is required.",
                    nameof(objEntity));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            objDbContext.Entry(objEntity).State =
                EntityState.Modified;

            await objDbContext.SaveChangesAsync(objToken);
        }
    }
}