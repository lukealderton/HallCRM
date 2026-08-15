using CRM.Core.Jobs.Domain;

namespace CRM.Core.Jobs.Abstractions
{
    public interface IJobRepository
    {
        /// <summary>
        /// Gets a job by its unique identifier.
        /// </summary>
        /// <param name="objJobId"></param>
        /// <param name="blnTracking"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Job?> GetJobByIdAsync(Guid objJobId, Boolean blnTracking = false, CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of jobs based on the provided search criteria.
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="enmStage"></param>
        /// <param name="objCompanyId"></param>
        /// <param name="blnIncludeArchived"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new job to the repository.
        /// </summary>
        /// <param name="objJob"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddJobAsync(Job objJob, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing Job in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of open jobs in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountOpenJobsAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total value of open jobs in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Decimal> GetOpenJobValueAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Gets a summary of jobs grouped by their stages.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<JobStageSummary>> GetStageSummaryAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of overdue jobs in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountOverdueJobsAsync(
            CancellationToken objToken = default);
    }
}