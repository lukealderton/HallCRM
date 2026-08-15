using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Jobs.Repositories
{
    public sealed class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public JobRepository(IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory = objDbContextFactory;
        }

        public async Task<Job?> GetJobByIdAsync(Guid objJobId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Job> objQuery = objDbContext.Jobs
                .Include(objJob => objJob.Entity)
                .Include(objJob => objJob.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objJob => objJob.Contact)
                    .ThenInclude(objContact => objContact!.Entity);

            if (!blnTracking)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return await objQuery.FirstOrDefaultAsync(objJob => objJob.Id == objJobId, objToken);
        }

        public async Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Job> objQuery = objDbContext.Jobs
                .AsNoTracking()
                .Include(objJob => objJob.Entity)
                .Include(objJob => objJob.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objJob => objJob.Contact)
                    .ThenInclude(objContact => objContact!.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery = objQuery.Where(objJob => !objJob.Entity.DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery = objQuery.Where(objJob => !objJob.Entity.ArchivedUtc.HasValue);
            }

            if (enmStage.HasValue)
            {
                objQuery = objQuery.Where(objJob => objJob.Stage == enmStage.Value);
            }

            if (objCompanyId.HasValue)
            {
                objQuery = objQuery.Where(objJob => objJob.CompanyId == objCompanyId.Value);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strKeyword = strSearch.Trim();

                objQuery = objQuery.Where(objJob =>
                    objJob.Name.Contains(strKeyword) ||
                    (objJob.Description != null && objJob.Description.Contains(strKeyword)) ||
                    (objJob.Source != null && objJob.Source.Contains(strKeyword)) ||
                    (objJob.Company != null && objJob.Company.Name.Contains(strKeyword)) ||
                    (objJob.Contact != null && objJob.Contact.Entity.DisplayName.Contains(strKeyword)));
            }

            return await objQuery
                .OrderByDescending(objJob => objJob.ExpectedCloseDateUtc.HasValue)
                .ThenBy(objJob => objJob.ExpectedCloseDateUtc)
                .ThenBy(objJob => objJob.Name)
                .ToListAsync(objToken);
        }

        public async Task AddJobAsync(Job objJob, CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);
            await objDbContext.Jobs.AddAsync(objJob, objToken);
        }

        public async Task SaveChangesAsync(CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);
            await objDbContext.SaveChangesAsync(objToken);
        }
    }
}