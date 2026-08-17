using CRM.Core.Services.Abstractions;
using CRM.Core.Services.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Services.Repositories
{
    public sealed class ServiceRepository : IServiceRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public ServiceRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory =
                objDbContextFactory;
        }

        public async Task<Service?> GetServiceByIdAsync(
            Guid objServiceId,
            Boolean blnTracking = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Service> objQuery =
                objDbContext.Services
                    .Include(
                        objService =>
                            objService.Entity);

            if (!blnTracking)
            {
                objQuery =
                    objQuery.AsNoTracking();
            }

            return await objQuery
                .FirstOrDefaultAsync(
                    objService =>
                        objService.Id ==
                        objServiceId,
                    objToken);
        }

        public async Task<List<Service>> GetServicesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Service> objQuery =
                objDbContext.Services
                    .AsNoTracking()
                    .Include(
                        objService =>
                            objService.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery =
                    objQuery.Where(
                        objService =>
                            !objService.Entity
                                .DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery =
                    objQuery.Where(
                        objService =>
                            !objService.Entity
                                .ArchivedUtc.HasValue);
            }

            if (!String.IsNullOrWhiteSpace(
                strSearch))
            {
                String strKeyword =
                    strSearch.Trim();

                objQuery =
                    objQuery.Where(
                        objService =>
                            objService.Name.Contains(
                                strKeyword) ||
                            (
                                objService.Description != null &&
                                objService.Description.Contains(
                                    strKeyword)
                            ) ||
                            (
                                objService.Notes != null &&
                                objService.Notes.Contains(
                                    strKeyword)
                            ));
            }

            return await objQuery
                .OrderBy(
                    objService =>
                        objService.Name)
                .ToListAsync(
                    objToken);
        }

        public async Task AddServiceAsync(
            Service objService,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            objDbContext.Entry(objService).State =
                EntityState.Added;

            objDbContext.Entry(objService.Entity).State =
                EntityState.Added;

            await objDbContext.SaveChangesAsync(
                objToken);
        }

        public async Task UpdateServiceAsync(
            Service objService,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            objDbContext.Entry(objService).State =
                EntityState.Modified;

            objDbContext.Entry(objService.Entity).State =
                EntityState.Modified;

            await objDbContext.SaveChangesAsync(
                objToken);
        }
    }
}