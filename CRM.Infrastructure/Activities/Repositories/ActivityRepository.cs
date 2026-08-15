using CRM.Core.Activities.Abstractions;
using CRM.Core.Activities.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Activities.Repositories
{
    public sealed class ActivityRepository : IActivityRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public ActivityRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory =
                objDbContextFactory;
        }

        ///<inheritdoc/>
        public async Task<Activity?> GetActivityByIdAsync(
            Guid objActivityId,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Activities
                .AsNoTracking()
                .Include(objActivity => objActivity.Entity)
                .Include(objActivity => objActivity.Company)
                    .ThenInclude(objCompany => objCompany!.Entity)
                .Include(objActivity => objActivity.Contact)
                    .ThenInclude(objContact => objContact!.Entity)
                .Include(objActivity => objActivity.Job)
                    .ThenInclude(objJob => objJob!.Entity)
                .FirstOrDefaultAsync(
                    objActivity =>
                        objActivity.Id == objActivityId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<Activity>> GetActivitiesAsync(
            String? strSearch = null,
            ActivityType? enmType = null,
            Boolean? blnCompleted = null,
            Guid? objCompanyId = null,
            Guid? objContactId = null,
            Guid? objJobId = null,
            Boolean blnOverdueOnly = false,
            Boolean blnDueTodayOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Activity> objQuery =
                objDbContext.Activities
                    .AsNoTracking()
                    .Include(objActivity => objActivity.Entity)
                    .Include(objActivity => objActivity.Company)
                        .ThenInclude(objCompany => objCompany!.Entity)
                    .Include(objActivity => objActivity.Contact)
                        .ThenInclude(objContact => objContact!.Entity)
                    .Include(objActivity => objActivity.Job)
                        .ThenInclude(objJob => objJob!.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery = objQuery.Where(objActivity =>
                    !objActivity.Entity.DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery = objQuery.Where(objActivity =>
                    !objActivity.Entity.ArchivedUtc.HasValue);
            }

            if (enmType.HasValue)
            {
                objQuery = objQuery.Where(objActivity =>
                    objActivity.Type == enmType.Value);
            }

            if (blnCompleted.HasValue)
            {
                if (blnCompleted.Value)
                {
                    objQuery = objQuery.Where(objActivity =>
                        objActivity.CompletedUtc.HasValue);
                }
                else
                {
                    objQuery = objQuery.Where(objActivity =>
                        !objActivity.CompletedUtc.HasValue);
                }
            }

            if (objCompanyId.HasValue)
            {
                objQuery = objQuery.Where(objActivity =>
                    objActivity.CompanyId ==
                    objCompanyId.Value);
            }

            if (objContactId.HasValue)
            {
                objQuery = objQuery.Where(objActivity =>
                    objActivity.ContactId ==
                    objContactId.Value);
            }

            if (objJobId.HasValue)
            {
                objQuery = objQuery.Where(objActivity =>
                    objActivity.JobId ==
                    objJobId.Value);
            }

            if (blnOverdueOnly)
            {
                DateTime dteTodayUtc =
                    DateTime.UtcNow.Date;

                objQuery = objQuery.Where(objActivity =>
                    !objActivity.CompletedUtc.HasValue &&
                    objActivity.DueUtc.HasValue &&
                    objActivity.DueUtc.Value < dteTodayUtc);
            }

            if (blnDueTodayOnly)
            {
                DateTime dteTodayUtc =
                    DateTime.UtcNow.Date;

                DateTime dteTomorrowUtc =
                    dteTodayUtc.AddDays(1);

                objQuery = objQuery.Where(objActivity =>
                    !objActivity.CompletedUtc.HasValue &&
                    objActivity.DueUtc.HasValue &&
                    objActivity.DueUtc.Value >= dteTodayUtc &&
                    objActivity.DueUtc.Value < dteTomorrowUtc);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strKeyword =
                    strSearch.Trim();

                objQuery = objQuery.Where(objActivity =>
                    objActivity.Subject.Contains(strKeyword) ||
                    (
                        objActivity.Description != null &&
                        objActivity.Description.Contains(strKeyword)
                    ) ||
                    (
                        objActivity.Company != null &&
                        objActivity.Company.Name.Contains(strKeyword)
                    ) ||
                    (
                        objActivity.Contact != null &&
                        objActivity.Contact.Entity.DisplayName.Contains(strKeyword)
                    ) ||
                    (
                        objActivity.Job != null &&
                        objActivity.Job.Name.Contains(strKeyword)
                    ));
            }

            return await objQuery
                .OrderBy(objActivity =>
                    objActivity.CompletedUtc.HasValue)
                .ThenByDescending(objActivity =>
                    objActivity.DueUtc.HasValue)
                .ThenBy(objActivity =>
                    objActivity.DueUtc)
                .ThenBy(objActivity =>
                    objActivity.Subject)
                .ToListAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task AddActivityAsync(
            Activity objActivity,
            CancellationToken objToken = default)
        {
            if (objActivity == null)
            {
                throw new ArgumentNullException(
                    nameof(objActivity));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            objDbContext.Entry(objActivity).State =
                EntityState.Added;

            objDbContext.Entry(objActivity.Entity).State =
                EntityState.Added;

            await objDbContext.SaveChangesAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task UpdateActivityAsync(
            Activity objActivity,
            CancellationToken objToken = default)
        {
            if (objActivity == null)
            {
                throw new ArgumentNullException(
                    nameof(objActivity));
            }

            if (objActivity.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Activity id is required.",
                    nameof(objActivity));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            objDbContext.Entry(objActivity).State =
                EntityState.Modified;

            objDbContext.Entry(objActivity.Entity).State =
                EntityState.Modified;

            await objDbContext.SaveChangesAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountOpenActivitiesAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Activities
                .AsNoTracking()
                .Where(objActivity =>
                    !objActivity.Entity.ArchivedUtc.HasValue &&
                    !objActivity.Entity.DeletedUtc.HasValue &&
                    !objActivity.CompletedUtc.HasValue)
                .CountAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountOverdueActivitiesAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            DateTime dteTodayUtc =
                DateTime.UtcNow.Date;

            return await objDbContext.Activities
                .AsNoTracking()
                .Where(objActivity =>
                    !objActivity.Entity.ArchivedUtc.HasValue &&
                    !objActivity.Entity.DeletedUtc.HasValue &&
                    !objActivity.CompletedUtc.HasValue &&
                    objActivity.DueUtc.HasValue &&
                    objActivity.DueUtc.Value < dteTodayUtc)
                .CountAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountDueTodayActivitiesAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            DateTime dteTodayUtc =
                DateTime.UtcNow.Date;

            DateTime dteTomorrowUtc =
                dteTodayUtc.AddDays(1);

            return await objDbContext.Activities
                .AsNoTracking()
                .Where(objActivity =>
                    !objActivity.Entity.ArchivedUtc.HasValue &&
                    !objActivity.Entity.DeletedUtc.HasValue &&
                    !objActivity.CompletedUtc.HasValue &&
                    objActivity.DueUtc.HasValue &&
                    objActivity.DueUtc.Value >= dteTodayUtc &&
                    objActivity.DueUtc.Value < dteTomorrowUtc)
                .CountAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountDueSoonActivitiesAsync(
            Int32 intDays = 7,
            CancellationToken objToken = default)
        {
            if (intDays <= 0)
            {
                return 0;
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            DateTime dteTomorrowUtc =
                DateTime.UtcNow.Date.AddDays(1);

            DateTime dteCutoffUtc =
                dteTomorrowUtc.AddDays(intDays);

            return await objDbContext.Activities
                .AsNoTracking()
                .Where(objActivity =>
                    !objActivity.Entity.ArchivedUtc.HasValue &&
                    !objActivity.Entity.DeletedUtc.HasValue &&
                    !objActivity.CompletedUtc.HasValue &&
                    objActivity.DueUtc.HasValue &&
                    objActivity.DueUtc.Value >= dteTomorrowUtc &&
                    objActivity.DueUtc.Value < dteCutoffUtc)
                .CountAsync(objToken);
        }
    }
}