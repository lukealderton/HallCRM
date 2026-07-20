using CRM.Core.Jobs.Domain;

namespace CRM.Core.Jobs.Abstractions
{
    public interface IJobService
    {
        /// <summary>
        /// Gets a job by its unique identifier.
        /// </summary>
        /// <param name="objJobId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Job?> GetJobByIdAsync(Guid objJobId, CancellationToken objToken = default);

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
        /// Adds a new job to the system.
        /// </summary>
        /// <param name="objJob"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Job> AddJobAsync(Job objJob, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing job in the system.
        /// </summary>
        /// <param name="objJob"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Job?> UpdateJobAsync(Job objJob, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Archives a job, marking it as inactive or hidden in the system.
        /// </summary>
        /// <param name="objJobId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived job, making it active or visible in the system again.
        /// </summary>
        /// <param name="objJobId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes a job from the system.
        /// </summary>
        /// <param name="objJobId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}