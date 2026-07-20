using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Jobs.Services
{
    public sealed class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository objJobRepository)
        {
            _jobRepository = objJobRepository;
        }

        ///<inheritdoc/>
        public Task<Job?> GetJobByIdAsync(Guid objJobId, CancellationToken objToken = default)
        {
            return _jobRepository.GetJobByIdAsync(objJobId, false, objToken);
        }

        ///<inheritdoc/>
        public Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _jobRepository.GetJobsAsync(
                strSearch,
                enmStage,
                objCompanyId,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Job> AddJobAsync(Job objJob, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(objJob.Name))
            {
                throw new ArgumentException("Job name is required.", nameof(objJob));
            }

            Guid objJobId = objJob.Id == Guid.Empty
                ? Guid.NewGuid()
                : objJob.Id;

            DateTime dteNow = DateTime.UtcNow;
            String strJobName = objJob.Name.Trim();

            objJob.Id = objJobId;
            objJob.Name = strJobName;
            objJob.Description = CleanString(objJob.Description);
            objJob.Source = CleanString(objJob.Source);
            objJob.Notes = CleanString(objJob.Notes);
            objJob.ProbabilityPercent = CleanProbability(objJob.ProbabilityPercent);

            objJob.Entity = new CrmEntity
            {
                Id = objJobId,
                EntityTypeId = (Int32)PredefinedEntityType.Job,
                DisplayName = strJobName,
                OwnerUserId = objUserId,
                CreatedUtc = dteNow,
                CreatedByUserId = objUserId
            };

            await _jobRepository.AddJobAsync(objJob, objToken);
            await _jobRepository.SaveChangesAsync(objToken);

            return objJob;
        }

        ///<inheritdoc/>
        public async Task<Job?> UpdateJobAsync(Job objJob, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (objJob.Id == Guid.Empty)
            {
                throw new ArgumentException("Job id is required.", nameof(objJob));
            }

            if (String.IsNullOrWhiteSpace(objJob.Name))
            {
                throw new ArgumentException("Job name is required.", nameof(objJob));
            }

            Job? objExistingJob = await _jobRepository.GetJobByIdAsync(objJob.Id, true, objToken);

            if (objExistingJob == null || objExistingJob.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow = DateTime.UtcNow;
            String strJobName = objJob.Name.Trim();

            objExistingJob.CompanyId = objJob.CompanyId;
            objExistingJob.ContactId = objJob.ContactId;
            objExistingJob.Name = strJobName;
            objExistingJob.Description = CleanString(objJob.Description);
            objExistingJob.Stage = objJob.Stage;
            objExistingJob.Value = objJob.Value;
            objExistingJob.ProbabilityPercent = CleanProbability(objJob.ProbabilityPercent);
            objExistingJob.ExpectedCloseDateUtc = objJob.ExpectedCloseDateUtc;
            objExistingJob.Source = CleanString(objJob.Source);
            objExistingJob.Notes = CleanString(objJob.Notes);

            objExistingJob.Entity.DisplayName = strJobName;
            objExistingJob.Entity.UpdatedUtc = dteNow;
            objExistingJob.Entity.UpdatedByUserId = objUserId;

            await _jobRepository.SaveChangesAsync(objToken);

            return objExistingJob;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Job? objJob = await _jobRepository.GetJobByIdAsync(objJobId, true, objToken);

            if (objJob == null || objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objJob.Entity.ArchivedUtc = dteNow;
            objJob.Entity.ArchivedByUserId = objUserId;
            objJob.Entity.UpdatedUtc = dteNow;
            objJob.Entity.UpdatedByUserId = objUserId;

            await _jobRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Job? objJob = await _jobRepository.GetJobByIdAsync(objJobId, true, objToken);

            if (objJob == null || objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objJob.Entity.ArchivedUtc = null;
            objJob.Entity.ArchivedByUserId = null;
            objJob.Entity.UpdatedUtc = dteNow;
            objJob.Entity.UpdatedByUserId = objUserId;

            await _jobRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteJobAsync(Guid objJobId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Job? objJob = await _jobRepository.GetJobByIdAsync(objJobId, true, objToken);

            if (objJob == null || objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objJob.Entity.DeletedUtc = dteNow;
            objJob.Entity.DeletedByUserId = objUserId;
            objJob.Entity.UpdatedUtc = dteNow;
            objJob.Entity.UpdatedByUserId = objUserId;

            await _jobRepository.SaveChangesAsync(objToken);

            return true;
        }

        /// <summary>
        /// Cleans a string value by trimming whitespace and returning null if the string is null or whitespace.
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private static String? CleanString(String? strValue)
        {
            if (String.IsNullOrWhiteSpace(strValue))
            {
                return null;
            }

            return strValue.Trim();
        }

        /// <summary>
        /// Cleans a probability percentage value by ensuring it is within the range of 0 to 100. If the value is null, it returns null. If the value is less than 0, it returns 0. If the value is greater than 100, it returns 100. Otherwise, it returns the original value.
        /// </summary>
        /// <param name="intProbabilityPercent"></param>
        /// <returns></returns>
        private static Int32? CleanProbability(Int32? intProbabilityPercent)
        {
            if (!intProbabilityPercent.HasValue)
            {
                return null;
            }

            if (intProbabilityPercent.Value < 0)
            {
                return 0;
            }

            if (intProbabilityPercent.Value > 100)
            {
                return 100;
            }

            return intProbabilityPercent.Value;
        }
    }
}