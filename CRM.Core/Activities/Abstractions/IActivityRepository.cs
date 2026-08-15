using CRM.Core.Activities.Domain;

namespace CRM.Core.Activities.Abstractions
{
    public interface IActivityRepository
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
            Boolean blnDueTodayOnly = false,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task AddActivityAsync(
            Activity objActivity,
            CancellationToken objToken = default);

        Task UpdateActivityAsync(
            Activity objActivity,
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