using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Jobs.Repositories
{
    public sealed class JobRepository : IJobRepository
    {
        private readonly CRMDbContext _context;

        public JobRepository(CRMDbContext objContext)
        {
            _context = objContext;
        }

        public Task<Job?> GetJobByIdAsync(Guid objJobId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            IQueryable<Job> objQuery = _context.Jobs
                .Include(objJob => objJob.Entity)
                .Include(objJob => objJob.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objJob => objJob.Contact)
                    .ThenInclude(objContact => objContact!.Entity);

            if (!blnTracking)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return objQuery.FirstOrDefaultAsync(objJob => objJob.Id == objJobId, objToken);
        }

        public Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<Job> objQuery = _context.Jobs
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

            return objQuery
                .OrderByDescending(objJob => objJob.ExpectedCloseDateUtc.HasValue)
                .ThenBy(objJob => objJob.ExpectedCloseDateUtc)
                .ThenBy(objJob => objJob.Name)
                .ToListAsync(objToken);
        }

        public Task AddJobAsync(Job objJob, CancellationToken objToken = default)
        {
            return _context.Jobs.AddAsync(objJob, objToken).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken objToken = default)
        {
            return _context.SaveChangesAsync(objToken);
        }
    }
}