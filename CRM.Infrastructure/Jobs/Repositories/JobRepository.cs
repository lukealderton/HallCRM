using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Jobs.Repositories
{
    public sealed class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public JobRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory =
                objDbContextFactory;
        }

        public async Task<Job?> GetJobByIdAsync(
            Guid objJobId,
            Boolean blnTracking = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Job> objQuery =
                objDbContext.Jobs
                    .Include(objJob =>
                        objJob.Entity)
                    .Include(objJob =>
                        objJob.Company)
                        .ThenInclude(objCompany =>
                            objCompany!.Entity)
                    .Include(objJob =>
                        objJob.Contact)
                        .ThenInclude(objContact =>
                            objContact!.Entity)
                    .Include(objJob =>
                        objJob.ServiceLinks)
                        .ThenInclude(objLink =>
                            objLink.Service);

            if (!blnTracking)
            {
                objQuery =
                    objQuery.AsNoTracking();
            }

            return await objQuery
                .FirstOrDefaultAsync(
                    objJob =>
                        objJob.Id == objJobId,
                    objToken);
        }

        public async Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Guid? objServiceId = null,
            Guid? objAssignedUserId = null,
            Boolean blnUnassignedOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            Boolean blnOverdueOnly = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            IQueryable<Job> objQuery =
                objDbContext.Jobs
                    .AsNoTracking()
                    .Include(objJob =>
                        objJob.Entity)
                    .Include(objJob =>
                        objJob.Company)
                        .ThenInclude(objCompany =>
                            objCompany!.Entity)
                    .Include(objJob =>
                        objJob.Contact)
                        .ThenInclude(objContact =>
                            objContact!.Entity)
                    .Include(objJob =>
                        objJob.ServiceLinks)
                        .ThenInclude(objLink =>
                            objLink.Service);

            if (!blnIncludeDeleted)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            !objJob.Entity
                                .DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            !objJob.Entity
                                .ArchivedUtc.HasValue);
            }

            if (enmStage.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            objJob.Stage ==
                            enmStage.Value);
            }

            if (objCompanyId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            objJob.CompanyId ==
                            objCompanyId.Value);
            }

            if (objServiceId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            objJob.ServiceLinks.Any(
                                objLink =>
                                    objLink.ServiceId ==
                                    objServiceId.Value));
            }

            if (objAssignedUserId.HasValue)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            objJob.AssignedUserId ==
                            objAssignedUserId.Value);
            }

            if (blnUnassignedOnly)
            {
                objQuery =
                    objQuery.Where(
                        objJob =>
                            !objJob.AssignedUserId.HasValue);
            }

            if (blnOverdueOnly)
            {
                DateTime dteTodayUtc = DateTime.UtcNow.Date;

                objQuery =
                    objQuery.Where(
                        objJob =>
                            !objJob.Entity
                                .ArchivedUtc.HasValue &&
                            objJob.Stage !=
                                JobStage.Paid &&
                            objJob.Stage !=
                                JobStage.Lost &&
                            objJob.ExpectedCloseDateUtc
                                .HasValue &&
                            objJob.ExpectedCloseDateUtc
                                .Value <
                                dteTodayUtc);
            }

            if (!String.IsNullOrWhiteSpace(
                strSearch))
            {
                String strKeyword =
                    strSearch.Trim();

                objQuery =
                    objQuery.Where(
                        objJob =>
                            objJob.Name.Contains(
                                strKeyword) ||
                            (
                                objJob.Description != null &&
                                objJob.Description.Contains(
                                    strKeyword)
                            ) ||
                            (
                                objJob.Source != null &&
                                objJob.Source.Contains(
                                    strKeyword)
                            ) ||
                            (
                                objJob.Company != null &&
                                objJob.Company.Name.Contains(
                                    strKeyword)
                            ) ||
                            (
                                objJob.Contact != null &&
                                objJob.Contact.Entity
                                    .DisplayName.Contains(
                                        strKeyword)
                            ) ||
                            objJob.ServiceLinks.Any(
                                objLink =>
                                    objLink.Service.Name
                                        .Contains(
                                            strKeyword))
                            || (objJob.AddressLine1 != null &&
                                objJob.AddressLine1.Contains(
                                    strKeyword))

                            || (objJob.AddressLine2 != null &&
                                objJob.AddressLine2.Contains(
                                    strKeyword))

                            || (objJob.Town != null &&
                                objJob.Town.Contains(
                                    strKeyword))

                            || (objJob.County != null &&
                                objJob.County.Contains(
                                    strKeyword))

                            || (objJob.Postcode != null &&
                                objJob.Postcode.Contains(
                                    strKeyword))

                            || (objJob.SiteContactName != null &&
                                objJob.SiteContactName.Contains(
                                    strKeyword))
                    );
            }

            return await objQuery
                .OrderByDescending(
                    objJob =>
                        objJob.ExpectedCloseDateUtc
                            .HasValue)
                .ThenBy(
                    objJob =>
                        objJob.ExpectedCloseDateUtc)
                .ThenBy(
                    objJob =>
                        objJob.Name)
                .ToListAsync(
                    objToken);
        }

        public async Task AddJobAsync(
            Job objJob,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            objDbContext.Entry(objJob).State =
                EntityState.Added;

            objDbContext.Entry(objJob.Entity).State =
                EntityState.Added;

            await objDbContext.SaveChangesAsync(
                objToken);
        }

        public async Task UpdateJobAsync(
            Job objJob,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            objDbContext.Entry(objJob).State =
                EntityState.Modified;

            objDbContext.Entry(objJob.Entity).State =
                EntityState.Modified;

            await objDbContext.SaveChangesAsync(
                objToken);
        }

        public async Task SetJobServicesAsync(
    Guid objJobId,
    IReadOnlyCollection<JobServiceLink> colServiceLinks,
    CancellationToken objToken = default)
        {
            await using CRMDbContext objContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            List<JobServiceLink> colExistingLinks =
                await objContext
                    .Set<JobServiceLink>()
                    .Where(
                        objLink =>
                            objLink.JobId ==
                            objJobId)
                    .ToListAsync(
                        objToken);

            HashSet<Guid> colRequestedServiceIds =
                colServiceLinks
                    .Select(
                        objLink =>
                            objLink.ServiceId)
                    .ToHashSet();

            List<JobServiceLink> colLinksToRemove =
                colExistingLinks
                    .Where(
                        objLink =>
                            !colRequestedServiceIds.Contains(
                                objLink.ServiceId))
                    .ToList();

            if (colLinksToRemove.Count > 0)
            {
                objContext
                    .Set<JobServiceLink>()
                    .RemoveRange(
                        colLinksToRemove);
            }

            foreach (JobServiceLink objRequestedLink
                in colServiceLinks)
            {
                JobServiceLink? objExistingLink =
                    colExistingLinks
                        .FirstOrDefault(
                            objLink =>
                                objLink.ServiceId ==
                                objRequestedLink.ServiceId);

                Decimal dcmQuantity =
                    objRequestedLink.Quantity <= 0m
                        ? 1m
                        : objRequestedLink.Quantity;

                if (objExistingLink != null)
                {
                    objExistingLink.Quantity =
                        dcmQuantity;

                    objExistingLink.UnitPrice =
                        objRequestedLink.UnitPrice;

                    continue;
                }

                objContext
                    .Set<JobServiceLink>()
                    .Add(
                        new JobServiceLink
                        {
                            JobId =
                                objJobId,

                            ServiceId =
                                objRequestedLink.ServiceId,

                            Quantity =
                                dcmQuantity,

                            UnitPrice =
                                objRequestedLink.UnitPrice
                        });
            }

            await objContext.SaveChangesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountOpenJobsAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            return await objDbContext.Jobs
                .AsNoTracking()
                .Where(objJob =>
                    !objJob.Entity.ArchivedUtc.HasValue &&
                    !objJob.Entity.DeletedUtc.HasValue &&
                    objJob.Stage != JobStage.Paid &&
                    objJob.Stage != JobStage.Lost)
                .CountAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Decimal> GetOpenJobValueAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            return await objDbContext.Jobs
                .AsNoTracking()
                .Where(objJob =>
                    !objJob.Entity.ArchivedUtc.HasValue &&
                    !objJob.Entity.DeletedUtc.HasValue &&
                    objJob.Stage != JobStage.Paid &&
                    objJob.Stage != JobStage.Lost)
                .SumAsync(
                    objJob =>
                        objJob.Value ?? 0m,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<JobStageSummary>> GetStageSummaryAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            return await objDbContext.Jobs
                .AsNoTracking()
                .Where(objJob =>
                    !objJob.Entity.ArchivedUtc.HasValue &&
                    !objJob.Entity.DeletedUtc.HasValue)
                .GroupBy(objJob =>
                    objJob.Stage)
                .Select(objGroup =>
                    new JobStageSummary
                    {
                        Stage =
                            objGroup.Key,

                        Count =
                            objGroup.Count(),

                        Value =
                            objGroup.Sum(
                                objJob =>
                                    objJob.Value ?? 0m)
                    })
                .OrderBy(objSummary =>
                    objSummary.Stage)
                .ToListAsync(
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountOverdueJobsAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(
                    objToken);

            DateTime dtmToday =
                DateTime.UtcNow.Date;

            return await objDbContext.Jobs
                .AsNoTracking()
                .Where(objJob =>
                    !objJob.Entity.ArchivedUtc.HasValue &&
                    !objJob.Entity.DeletedUtc.HasValue &&
                    objJob.Stage != JobStage.Paid &&
                    objJob.Stage != JobStage.Lost &&
                    objJob.ExpectedCloseDateUtc.HasValue &&
                    objJob.ExpectedCloseDateUtc.Value <
                        dtmToday)
                .CountAsync(
                    objToken);
        }
    }
}