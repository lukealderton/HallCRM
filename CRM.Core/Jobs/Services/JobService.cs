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
            Guid? objAssignedUserId = null,
            Boolean blnUnassignedOnly = false,
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
                objAssignedUserId,
                blnUnassignedOnly,
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

            List<JobServiceLink> colServiceLinks =
                CleanServiceLinks(
                    objJobId,
                    objJob.ServiceLinks);

            ApplyServiceTotal(
                objJob,
                colServiceLinks);

            objJob.Name =
                strJobName;

            objJob.Description =
                CleanString(
                    objJob.Description);

            objJob.Source =
                CleanString(
                    objJob.Source);

            objJob.Notes =
                CleanString(
                    objJob.Notes);

            objJob.ProbabilityPercent =
                CleanProbability(
                    objJob.ProbabilityPercent);

            objJob.AddressLine1 =
                CleanString(
                    objJob.AddressLine1);

            objJob.AddressLine2 =
                CleanString(
                    objJob.AddressLine2);

            objJob.Town =
                CleanString(
                    objJob.Town);

            objJob.County =
                CleanString(
                    objJob.County);

            objJob.Postcode =
                CleanPostcode(
                    objJob.Postcode);

            objJob.SiteContactName =
                CleanString(
                    objJob.SiteContactName);

            objJob.SiteContactPhone =
                CleanString(
                    objJob.SiteContactPhone);

            objJob.AccessNotes =
                CleanString(
                    objJob.AccessNotes);

            /*
             * Service links are persisted separately by
             * SetJobServicesAsync().
             *
             * Do not pass the navigation graph into AddJobAsync()
             * otherwise the repository may attempt to persist the
             * join records itself.
             */
            objJob.ServiceLinks =
                [];

            objJob.Entity =
                new CrmEntity
                {
                    Id =
                        objJobId,

                    EntityTypeId =
                        (Int32)PredefinedEntityType.Job,

                    DisplayName =
                        strJobName,

                    OwnerUserId =
                        objUserId,

                    CreatedUtc =
                        dteNow,

                    CreatedByUserId =
                        objUserId
                };

            await _jobRepository.AddJobAsync(
                objJob,
                objToken);

            await _jobRepository.SetJobServicesAsync(
                objJobId,
                colServiceLinks,
                objToken);

            Job? objSavedJob =
                await _jobRepository.GetJobByIdAsync(
                    objJobId,
                    false,
                    objToken);

            return objSavedJob ??
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

            /*
             * Capture the incoming service lines before loading
             * the existing Job. They are persisted separately.
             */
            List<JobServiceLink> colServiceLinks =
                CleanServiceLinks(
                    objJob.Id,
                    objJob.ServiceLinks);

            ApplyServiceTotal(
                objJob,
                colServiceLinks);

            Job? objExistingJob =
                await _jobRepository.GetJobByIdAsync(
                    objJob.Id,
                    true,
                    objToken);

            if (objExistingJob == null ||
                objExistingJob.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow =
                DateTime.UtcNow;

            String strJobName =
                objJob.Name.Trim();

            objExistingJob.CompanyId =
                objJob.CompanyId;

            objExistingJob.ContactId =
                objJob.ContactId;

            objExistingJob.AssignedUserId =
                objJob.AssignedUserId;

            objExistingJob.Name =
                strJobName;

            objExistingJob.Description =
                CleanString(
                    objJob.Description);

            objExistingJob.Stage =
                objJob.Stage;

            objExistingJob.Value =
                objJob.Value;

            objExistingJob.ProbabilityPercent =
                CleanProbability(
                    objJob.ProbabilityPercent);

            objExistingJob.ExpectedCloseDateUtc =
                objJob.ExpectedCloseDateUtc;

            objExistingJob.Source =
                CleanString(
                    objJob.Source);

            objExistingJob.Notes =
                CleanString(
                    objJob.Notes);

            objExistingJob.AddressLine1 =
                CleanString(
                    objJob.AddressLine1);

            objExistingJob.AddressLine2 =
                CleanString(
                    objJob.AddressLine2);

            objExistingJob.Town =
                CleanString(
                    objJob.Town);

            objExistingJob.County =
                CleanString(
                    objJob.County);

            objExistingJob.Postcode =
                CleanPostcode(
                    objJob.Postcode);

            objExistingJob.SiteContactName =
                CleanString(
                    objJob.SiteContactName);

            objExistingJob.SiteContactPhone =
                CleanString(
                    objJob.SiteContactPhone);

            objExistingJob.AccessNotes =
                CleanString(
                    objJob.AccessNotes);

            objExistingJob.Entity.DisplayName =
                strJobName;

            objExistingJob.Entity.UpdatedUtc =
                dteNow;

            objExistingJob.Entity.UpdatedByUserId =
                objUserId;

            /*
             * The join table is managed separately.
             *
             * Remove the loaded navigation objects before passing
             * the detached Job graph into the repository's update.
             */
            objExistingJob.ServiceLinks =
                [];

            await _jobRepository.UpdateJobAsync(
                objExistingJob,
                objToken);

            await _jobRepository.SetJobServicesAsync(
                objJob.Id,
                colServiceLinks,
                objToken);

            return await _jobRepository.GetJobByIdAsync(
                objJob.Id,
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
                    true,
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

            /*
             * Service links are not changing during an archive.
             * Avoid attaching the loaded relationship graph.
             */
            objJob.ServiceLinks =
                [];

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
                    true,
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

            objJob.ServiceLinks =
                [];

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
                    true,
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

            objJob.ServiceLinks =
                [];

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

        /// <summary>
        /// Creates a clean snapshot of the service lines supplied
        /// for a job.
        /// </summary>
        private static List<JobServiceLink> CleanServiceLinks(
            Guid objJobId,
            IEnumerable<JobServiceLink>? colServiceLinks)
        {
            if (colServiceLinks == null)
            {
                return [];
            }

            return colServiceLinks
                .Where(objLink =>
                    objLink.ServiceId != Guid.Empty)
                .GroupBy(objLink =>
                    objLink.ServiceId)
                .Select(objGroup =>
                {
                    JobServiceLink objLink =
                        objGroup.First();

                    return new JobServiceLink
                    {
                        JobId =
                            objJobId,

                        ServiceId =
                            objLink.ServiceId,

                        Quantity =
                            CleanQuantity(
                                objLink.Quantity),

                        UnitPrice =
                            CleanUnitPrice(
                                objLink.UnitPrice)
                    };
                })
                .ToList();
        }

        /// <summary>
        /// Updates the headline job value when one or more service
        /// lines have explicit pricing.
        ///
        /// If no service line has a price, the manually entered job
        /// value is preserved.
        /// </summary>
        private static void ApplyServiceTotal(
            Job objJob,
            IReadOnlyCollection<JobServiceLink> colServiceLinks)
        {
            List<JobServiceLink> colPricedLinks =
                colServiceLinks
                    .Where(objLink =>
                        objLink.UnitPrice.HasValue)
                    .ToList();

            if (colPricedLinks.Count == 0)
            {
                return;
            }

            objJob.Value =
                colPricedLinks.Sum(
                    objLink =>
                        objLink.Quantity *
                        objLink.UnitPrice!.Value);
        }

        /// <summary>
        /// Ensures a service quantity is greater than zero and
        /// stores no more than two decimal places.
        /// </summary>
        private static Decimal CleanQuantity(
            Decimal dcmQuantity)
        {
            if (dcmQuantity <= 0m)
            {
                return 1m;
            }

            return Decimal.Round(
                dcmQuantity,
                2,
                MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Cleans a service unit price while retaining null to mean
        /// that no price has been entered.
        /// </summary>
        private static Decimal? CleanUnitPrice(
            Decimal? dcmUnitPrice)
        {
            if (!dcmUnitPrice.HasValue)
            {
                return null;
            }

            Decimal dcmValue =
                dcmUnitPrice.Value < 0m
                    ? 0m
                    : dcmUnitPrice.Value;

            return Decimal.Round(
                dcmValue,
                2,
                MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Cleans a string by trimming whitespace and returning
        /// null when no meaningful value exists.
        /// </summary>
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

        /// <summary>
        /// Cleans and normalises a UK postcode-style value.
        /// </summary>
        private static String? CleanPostcode(
            String? strPostcode)
        {
            String? strValue =
                CleanString(
                    strPostcode);

            return strValue?.ToUpperInvariant();
        }

        /// <summary>
        /// Ensures probability remains between zero and one hundred.
        /// </summary>
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
    }
}