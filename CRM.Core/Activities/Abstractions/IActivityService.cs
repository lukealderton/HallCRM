using CRM.Core.Activities.Domain;

namespace CRM.Core.Activities.Abstractions
{
    public interface IActivityService
    {
        Task<Activity?> GetActivityByIdAsync(
            Guid objActivityId,
            CancellationToken objToken = default);

        Task<List<Activity>> GetActivitiesAsync(
            String? strSearch = null,
            ActivityType? enmType = null,
            Boolean? blnCompleted = null,
            Guid? objCompanyId = null,
            Guid? objContactId = null,
            Guid? objJobId = null,
            Boolean blnOverdueOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task<Activity> AddActivityAsync(
            Activity objActivity,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Activity?> UpdateActivityAsync(
            Activity objActivity,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> CompleteActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> ReopenActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> ArchiveActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> RestoreActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Boolean> DeleteActivityAsync(
            Guid objActivityId,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        Task<Int32> CountOpenActivitiesAsync(
            CancellationToken objToken = default);

        Task<Int32> CountOverdueActivitiesAsync(
            CancellationToken objToken = default);

        Task<Int32> CountDueTodayActivitiesAsync(
            CancellationToken objToken = default);

        Task<Int32> CountDueSoonActivitiesAsync(
            Int32 intDays = 7,
            CancellationToken objToken = default);
    }
}