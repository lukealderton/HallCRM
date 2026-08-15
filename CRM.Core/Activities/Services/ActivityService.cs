using CRM.Core.Activities.Abstractions;
using CRM.Core.Activities.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Activities.Services
{
    public sealed class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(
            IActivityRepository objActivityRepository)
        {
            _activityRepository =
                objActivityRepository;
        }

        ///<inheritdoc/>
        public Task<Activity?> GetActivityByIdAsync(
            Guid objActivityId,
            CancellationToken objToken = default)
        {
            if (objActivityId == Guid.Empty)
            {
                return Task.FromResult<Activity?>(null);
            }

            return _activityRepository.GetActivityByIdAsync(
                objActivityId,
                objToken);
        }

        ///<inheritdoc/>
        public Task<List<Activity>> GetActivitiesAsync(
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
            return _activityRepository.GetActivitiesAsync(
                strSearch,
                enmType,
                blnCompleted,
                objCompanyId,
                objContactId,
                objJobId,
                blnOverdueOnly,
                blnDueTodayOnly,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Activity> AddActivityAsync(
            Activity objActivity,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objActivity == null)
            {
                throw new ArgumentNullException(
                    nameof(objActivity));
            }

            if (String.IsNullOrWhiteSpace(objActivity.Subject))
            {
                throw new ArgumentException(
                    "Activity subject is required.",
                    nameof(objActivity));
            }

            Guid objActivityId =
                objActivity.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objActivity.Id;

            DateTime dteNow =
                DateTime.UtcNow;

            String strSubject =
                objActivity.Subject.Trim();

            objActivity.Id =
                objActivityId;

            objActivity.Subject =
                strSubject;

            objActivity.Description =
                CleanString(objActivity.Description);

            objActivity.Entity =
                new CrmEntity
                {
                    Id = objActivityId,
                    EntityTypeId =
                        (Int32)PredefinedEntityType.Activity,
                    DisplayName = strSubject,
                    OwnerUserId = objUserId,
                    CreatedUtc = dteNow,
                    CreatedByUserId = objUserId
                };

            await _activityRepository.AddActivityAsync(
                objActivity,
                objToken);

            return objActivity;
        }

        ///<inheritdoc/>
        public async Task<Activity?> UpdateActivityAsync(
            Activity objActivity,
            Guid? objUserId = null,
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

            if (String.IsNullOrWhiteSpace(objActivity.Subject))
            {
                throw new ArgumentException(
                    "Activity subject is required.",
                    nameof(objActivity));
            }

            Activity? objExistingActivity =
                await _activityRepository.GetActivityByIdAsync(
                    objActivity.Id,
                    objToken);

            if (objExistingActivity == null ||
                objExistingActivity.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            String strSubject =
                objActivity.Subject.Trim();

            objExistingActivity.CompanyId =
                objActivity.CompanyId;

            objExistingActivity.ContactId =
                objActivity.ContactId;

            objExistingActivity.JobId =
                objActivity.JobId;

            objExistingActivity.AssignedUserId =
                objActivity.AssignedUserId;

            objExistingActivity.Type =
                objActivity.Type;

            objExistingActivity.Subject =
                strSubject;

            objExistingActivity.Description =
                CleanString(objActivity.Description);

            objExistingActivity.DueUtc =
                objActivity.DueUtc;

            objExistingActivity.CompletedUtc =
                objActivity.CompletedUtc;

            objExistingActivity.Entity.DisplayName =
                strSubject;

            objExistingActivity.Entity.UpdatedUtc =
                dteNow;

            objExistingActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objExistingActivity,
                objToken);

            return objExistingActivity;
        }

        ///<inheritdoc/>
        public async Task<Boolean> CompleteActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Activity? objActivity =
                await GetActiveActivityAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null)
            {
                return false;
            }

            if (objActivity.CompletedUtc.HasValue)
            {
                return true;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objActivity.CompletedUtc =
                dteNow;

            objActivity.Entity.UpdatedUtc =
                dteNow;

            objActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objActivity,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ReopenActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Activity? objActivity =
                await GetActiveActivityAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null)
            {
                return false;
            }

            if (!objActivity.CompletedUtc.HasValue)
            {
                return true;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objActivity.CompletedUtc =
                null;

            objActivity.Entity.UpdatedUtc =
                dteNow;

            objActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objActivity,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Activity? objActivity =
                await GetActiveActivityAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null)
            {
                return false;
            }

            if (objActivity.Entity.ArchivedUtc.HasValue)
            {
                return true;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objActivity.Entity.ArchivedUtc =
                dteNow;

            objActivity.Entity.ArchivedByUserId =
                objUserId;

            objActivity.Entity.UpdatedUtc =
                dteNow;

            objActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objActivity,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objActivityId == Guid.Empty)
            {
                return false;
            }

            Activity? objActivity =
                await _activityRepository.GetActivityByIdAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null ||
                objActivity.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            if (!objActivity.Entity.ArchivedUtc.HasValue)
            {
                return true;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objActivity.Entity.ArchivedUtc =
                null;

            objActivity.Entity.ArchivedByUserId =
                null;

            objActivity.Entity.UpdatedUtc =
                dteNow;

            objActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objActivity,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Activity? objActivity =
                await GetActiveActivityAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objActivity.Entity.DeletedUtc =
                dteNow;

            objActivity.Entity.DeletedByUserId =
                objUserId;

            objActivity.Entity.UpdatedUtc =
                dteNow;

            objActivity.Entity.UpdatedByUserId =
                objUserId;

            await _activityRepository.UpdateActivityAsync(
                objActivity,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public Task<Int32> CountOpenActivitiesAsync(
            CancellationToken objToken = default)
        {
            return _activityRepository.CountOpenActivitiesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<Int32> CountOverdueActivitiesAsync(
            CancellationToken objToken = default)
        {
            return _activityRepository.CountOverdueActivitiesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<Int32> CountDueTodayActivitiesAsync(
            CancellationToken objToken = default)
        {
            return _activityRepository.CountDueTodayActivitiesAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<Int32> CountDueSoonActivitiesAsync(
            Int32 intDays = 7,
            CancellationToken objToken = default)
        {
            if (intDays <= 0)
            {
                return Task.FromResult(0);
            }

            return _activityRepository.CountDueSoonActivitiesAsync(
                intDays,
                objToken);
        }

        private async Task<Activity?> GetActiveActivityAsync(
            Guid objActivityId,
            CancellationToken objToken)
        {
            if (objActivityId == Guid.Empty)
            {
                return null;
            }

            Activity? objActivity =
                await _activityRepository.GetActivityByIdAsync(
                    objActivityId,
                    objToken);

            if (objActivity == null ||
                objActivity.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            return objActivity;
        }

        private static String? CleanString(
            String? strValue)
        {
            if (String.IsNullOrWhiteSpace(strValue))
            {
                return null;
            }

            return strValue.Trim();
        }
    }
}