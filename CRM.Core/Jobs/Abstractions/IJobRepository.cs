using CRM.Core.Jobs.Domain;

namespace CRM.Core.Jobs.Abstractions
{
    public interface IJobRepository
    {
        /// <summary>
        /// Gets a job by its unique identifier.
        /// </summary>
        Task<Job?> GetJobByIdAsync(
            Guid objJobId,
            Boolean blnTracking = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of jobs based on the provided search criteria.
        /// </summary>
        Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            Boolean blnOverdueOnly = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new job to the repository.
        /// </summary>
        Task AddJobAsync(
            Job objJob,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing job in the repository.
        /// </summary>
        Task UpdateJobAsync(
            Job objJob,
            CancellationToken objToken = default);

        /// <summary>
        /// Sets the services linked to a job.
        /// </summary>
        Task SetJobServicesAsync(
            Guid objJobId,
            IReadOnlyCollection<Guid> colServiceIds,
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of open jobs in the repository.
        /// </summary>
        Task<Int32> CountOpenJobsAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total value of open jobs in the repository.
        /// </summary>
        Task<Decimal> GetOpenJobValueAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Gets a summary of jobs grouped by their stages.
        /// </summary>
        Task<List<JobStageSummary>> GetStageSummaryAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of overdue jobs in the repository.
        /// </summary>
        Task<Int32> CountOverdueJobsAsync(
            CancellationToken objToken = default);
    }
}