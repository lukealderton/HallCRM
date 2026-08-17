using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Abstractions;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Jobs.Services
{
    public sealed class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(
            IJobRepository objJobRepository)
        {
            _jobRepository =
                objJobRepository;
        }

        ///<inheritdoc/>
        public Task<Job?> GetJobByIdAsync(
            Guid objJobId,
            CancellationToken objToken = default)
        {
            return _jobRepository.GetJobByIdAsync(
                objJobId,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public Task<List<Job>> GetJobsAsync(
            String? strSearch = null,
            JobStage? enmStage = null,
            Guid? objCompanyId = null,
            Guid? objServiceId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            Boolean blnOverdueOnly = false,
            CancellationToken objToken = default)
        {
            return _jobRepository.GetJobsAsync(
                strSearch,
                enmStage,
                objCompanyId,
                objServiceId,
                blnIncludeArchived,
                blnIncludeDeleted,
                blnOverdueOnly,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Job> AddJobAsync(
            Job objJob,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(
                objJob.Name))
            {
                throw new ArgumentException(
                    "Job name is required.",
                    nameof(objJob));
            }

            Guid[] colServiceIds =
                [.. objJob.ServiceLinks
                    .Select(objLink =>
                        objLink.ServiceId)
                    .Where(objServiceId =>
                        objServiceId != Guid.Empty)
                    .Distinct()];

            Guid objJobId =
                objJob.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objJob.Id;

            DateTime dteNow =
                DateTime.UtcNow;

            String strJobName =
                objJob.Name.Trim();

            objJob.Id =
                objJobId;

            objJob.Name =
                strJobName;

            objJob.Description =
                CleanString(objJob.Description);

            objJob.Source =
                CleanString(objJob.Source);

            objJob.Notes =
                CleanString(objJob.Notes);

            objJob.AddressLine1 =
                CleanString(objJob.AddressLine1);

            objJob.AddressLine2 =
                CleanString(
                    objJob.AddressLine2);

            objJob.Town =
                CleanString(objJob.Town);

            objJob.County =
                CleanString(objJob.County);

            objJob.Postcode =
                CleanPostcode(objJob.Postcode);

            objJob.SiteContactName =
                CleanString(objJob.SiteContactName);

            objJob.SiteContactPhone =
                CleanString(objJob.SiteContactPhone);

            objJob.AccessNotes =
                CleanString(objJob.AccessNotes);

            objJob.ProbabilityPercent =
                CleanProbability(objJob.ProbabilityPercent);

            /*
             * Links are persisted separately so that
             * EF doesn't attempt to add Service records.
             */
            objJob.ServiceLinks = [];

            objJob.Entity =
                new CrmEntity
                {
                    Id = objJobId,

                    EntityTypeId = (Int32)PredefinedEntityType.Job,
                    DisplayName = strJobName,
                    OwnerUserId = objUserId,
                    CreatedUtc = dteNow,
                    CreatedByUserId = objUserId
                };

            await _jobRepository.AddJobAsync(
                objJob,
                objToken);

            await _jobRepository.SetJobServicesAsync(
                objJobId,
                colServiceIds,
                objToken);

            /*
             * Return a fully populated record so callers
             * immediately have the ServiceLinks.
             */
            Job? objCreatedJob =
                await _jobRepository.GetJobByIdAsync(
                    objJobId,
                    false,
                    objToken);

            return objCreatedJob ??
                   objJob;
        }

        ///<inheritdoc/>
        public async Task<Job?> UpdateJobAsync(
            Job objJob,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objJob.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Job id is required.",
                    nameof(objJob));
            }

            if (String.IsNullOrWhiteSpace(
                objJob.Name))
            {
                throw new ArgumentException(
                    "Job name is required.",
                    nameof(objJob));
            }

            Guid[] colServiceIds =
                [.. objJob.ServiceLinks
                    .Select(objLink =>
                        objLink.ServiceId)
                    .Where(objServiceId =>
                        objServiceId != Guid.Empty)
                    .Distinct()];

            Job? objExistingJob =
                await _jobRepository.GetJobByIdAsync(
                    objJob.Id,
                    false,
                    objToken);

            if (objExistingJob == null ||
                objExistingJob.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow =
                DateTime.UtcNow;

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

            objExistingJob.AddressLine1 = CleanString(objJob.AddressLine1);
            objExistingJob.AddressLine2 = CleanString(objJob.AddressLine2);
            objExistingJob.Town = CleanString(objJob.Town);
            objExistingJob.County = CleanString(objJob.County);
            objExistingJob.Postcode = CleanPostcode(objJob.Postcode);
            objExistingJob.SiteContactName = CleanString(objJob.SiteContactName);
            objExistingJob.SiteContactPhone = CleanString(objJob.SiteContactPhone);
            objExistingJob.AccessNotes = CleanString(objJob.AccessNotes);

            objExistingJob.Entity.DisplayName = strJobName;
            objExistingJob.Entity.UpdatedUtc = dteNow;
            objExistingJob.Entity.UpdatedByUserId = objUserId;

            /*
             * Existing links were loaded for reading,
             * but relationship changes are persisted by
             * SetJobServicesAsync().
             */
            objExistingJob.ServiceLinks = [];

            await _jobRepository.UpdateJobAsync(
                objExistingJob,
                objToken);

            await _jobRepository.SetJobServicesAsync(
                objExistingJob.Id,
                colServiceIds,
                objToken);

            return await _jobRepository.GetJobByIdAsync(
                objExistingJob.Id,
                false,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveJobAsync(
            Guid objJobId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Job? objJob =
                await _jobRepository.GetJobByIdAsync(
                    objJobId,
                    false,
                    objToken);

            if (objJob == null ||
                objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objJob.Entity.ArchivedUtc =
                dteNow;

            objJob.Entity.ArchivedByUserId =
                objUserId;

            objJob.Entity.UpdatedUtc =
                dteNow;

            objJob.Entity.UpdatedByUserId =
                objUserId;

            await _jobRepository.UpdateJobAsync(
                objJob,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreJobAsync(
            Guid objJobId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Job? objJob =
                await _jobRepository.GetJobByIdAsync(
                    objJobId,
                    false,
                    objToken);

            if (objJob == null ||
                objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objJob.Entity.ArchivedUtc =
                null;

            objJob.Entity.ArchivedByUserId =
                null;

            objJob.Entity.UpdatedUtc =
                dteNow;

            objJob.Entity.UpdatedByUserId =
                objUserId;

            await _jobRepository.UpdateJobAsync(
                objJob,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteJobAsync(
            Guid objJobId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Job? objJob =
                await _jobRepository.GetJobByIdAsync(
                    objJobId,
                    false,
                    objToken);

            if (objJob == null ||
                objJob.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            objJob.Entity.DeletedUtc =
                dteNow;

            objJob.Entity.DeletedByUserId =
                objUserId;

            objJob.Entity.UpdatedUtc =
                dteNow;

            objJob.Entity.UpdatedByUserId =
                objUserId;

            await _jobRepository.UpdateJobAsync(
                objJob,
                objToken);

            return true;
        }

        ///<inheritdoc/>
        public Task<Int32> CountOpenJobsAsync(
            CancellationToken objToken = default)
        {
            return _jobRepository.CountOpenJobsAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<Decimal> GetOpenJobValueAsync(
            CancellationToken objToken = default)
        {
            return _jobRepository.GetOpenJobValueAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<List<JobStageSummary>> GetStageSummaryAsync(
            CancellationToken objToken = default)
        {
            return _jobRepository.GetStageSummaryAsync(
                objToken);
        }

        ///<inheritdoc/>
        public Task<Int32> CountOverdueJobsAsync(
            CancellationToken objToken = default)
        {
            return _jobRepository.CountOverdueJobsAsync(
                objToken);
        }

        private static String? CleanString(
            String? strValue)
        {
            if (String.IsNullOrWhiteSpace(
                strValue))
            {
                return null;
            }

            return strValue.Trim();
        }

        private static Int32? CleanProbability(
            Int32? intProbabilityPercent)
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

        private static String? CleanPostcode(String? strPostcode)
        {
            String? strValue = CleanString(strPostcode);

            return strValue?.ToUpperInvariant();
        }
    }
}