using CRM.Core.Entities.Domain;
using CRM.Core.Opportunities.Abstractions;
using CRM.Core.Opportunities.Domain;

namespace CRM.Core.Opportunities.Services
{
    public sealed class OpportunityService : IOpportunityService
    {
        private readonly IOpportunityRepository _opportunityRepository;

        public OpportunityService(IOpportunityRepository objOpportunityRepository)
        {
            _opportunityRepository = objOpportunityRepository;
        }

        ///<inheritdoc/>
        public Task<Opportunity?> GetOpportunityByIdAsync(Guid objOpportunityId, CancellationToken objToken = default)
        {
            return _opportunityRepository.GetOpportunityByIdAsync(objOpportunityId, false, objToken);
        }

        ///<inheritdoc/>
        public Task<List<Opportunity>> GetOpportunitiesAsync(
            String? strSearch = null,
            OpportunityStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _opportunityRepository.GetOpportunitiesAsync(
                strSearch,
                enmStage,
                objCompanyId,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Opportunity> AddOpportunityAsync(Opportunity objOpportunity, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(objOpportunity.Name))
            {
                throw new ArgumentException("Opportunity name is required.", nameof(objOpportunity));
            }

            Guid objOpportunityId = objOpportunity.Id == Guid.Empty
                ? Guid.NewGuid()
                : objOpportunity.Id;

            DateTime dteNow = DateTime.UtcNow;
            String strOpportunityName = objOpportunity.Name.Trim();

            objOpportunity.Id = objOpportunityId;
            objOpportunity.Name = strOpportunityName;
            objOpportunity.Description = CleanString(objOpportunity.Description);
            objOpportunity.Source = CleanString(objOpportunity.Source);
            objOpportunity.Notes = CleanString(objOpportunity.Notes);
            objOpportunity.ProbabilityPercent = CleanProbability(objOpportunity.ProbabilityPercent);

            objOpportunity.Entity = new CrmEntity
            {
                Id = objOpportunityId,
                EntityTypeId = (Int32)PredefinedEntityType.Opportunity,
                DisplayName = strOpportunityName,
                OwnerUserId = objUserId,
                CreatedUtc = dteNow,
                CreatedByUserId = objUserId
            };

            await _opportunityRepository.AddOpportunityAsync(objOpportunity, objToken);
            await _opportunityRepository.SaveChangesAsync(objToken);

            return objOpportunity;
        }

        ///<inheritdoc/>
        public async Task<Opportunity?> UpdateOpportunityAsync(Opportunity objOpportunity, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (objOpportunity.Id == Guid.Empty)
            {
                throw new ArgumentException("Opportunity id is required.", nameof(objOpportunity));
            }

            if (String.IsNullOrWhiteSpace(objOpportunity.Name))
            {
                throw new ArgumentException("Opportunity name is required.", nameof(objOpportunity));
            }

            Opportunity? objExistingOpportunity = await _opportunityRepository.GetOpportunityByIdAsync(objOpportunity.Id, true, objToken);

            if (objExistingOpportunity == null || objExistingOpportunity.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow = DateTime.UtcNow;
            String strOpportunityName = objOpportunity.Name.Trim();

            objExistingOpportunity.CompanyId = objOpportunity.CompanyId;
            objExistingOpportunity.ContactId = objOpportunity.ContactId;
            objExistingOpportunity.Name = strOpportunityName;
            objExistingOpportunity.Description = CleanString(objOpportunity.Description);
            objExistingOpportunity.Stage = objOpportunity.Stage;
            objExistingOpportunity.Value = objOpportunity.Value;
            objExistingOpportunity.ProbabilityPercent = CleanProbability(objOpportunity.ProbabilityPercent);
            objExistingOpportunity.ExpectedCloseDateUtc = objOpportunity.ExpectedCloseDateUtc;
            objExistingOpportunity.Source = CleanString(objOpportunity.Source);
            objExistingOpportunity.Notes = CleanString(objOpportunity.Notes);

            objExistingOpportunity.Entity.DisplayName = strOpportunityName;
            objExistingOpportunity.Entity.UpdatedUtc = dteNow;
            objExistingOpportunity.Entity.UpdatedByUserId = objUserId;

            await _opportunityRepository.SaveChangesAsync(objToken);

            return objExistingOpportunity;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Opportunity? objOpportunity = await _opportunityRepository.GetOpportunityByIdAsync(objOpportunityId, true, objToken);

            if (objOpportunity == null || objOpportunity.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objOpportunity.Entity.ArchivedUtc = dteNow;
            objOpportunity.Entity.ArchivedByUserId = objUserId;
            objOpportunity.Entity.UpdatedUtc = dteNow;
            objOpportunity.Entity.UpdatedByUserId = objUserId;

            await _opportunityRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Opportunity? objOpportunity = await _opportunityRepository.GetOpportunityByIdAsync(objOpportunityId, true, objToken);

            if (objOpportunity == null || objOpportunity.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objOpportunity.Entity.ArchivedUtc = null;
            objOpportunity.Entity.ArchivedByUserId = null;
            objOpportunity.Entity.UpdatedUtc = dteNow;
            objOpportunity.Entity.UpdatedByUserId = objUserId;

            await _opportunityRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Opportunity? objOpportunity = await _opportunityRepository.GetOpportunityByIdAsync(objOpportunityId, true, objToken);

            if (objOpportunity == null || objOpportunity.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objOpportunity.Entity.DeletedUtc = dteNow;
            objOpportunity.Entity.DeletedByUserId = objUserId;
            objOpportunity.Entity.UpdatedUtc = dteNow;
            objOpportunity.Entity.UpdatedByUserId = objUserId;

            await _opportunityRepository.SaveChangesAsync(objToken);

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