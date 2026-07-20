using CRM.Core.Opportunities.Domain;

namespace CRM.Core.Opportunities.Abstractions
{
    public interface IOpportunityService
    {
        /// <summary>
        /// Gets an opportunity by its unique identifier.
        /// </summary>
        /// <param name="objOpportunityId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Opportunity?> GetOpportunityByIdAsync(Guid objOpportunityId, CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of opportunities based on the provided search criteria.
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="enmStage"></param>
        /// <param name="objCompanyId"></param>
        /// <param name="blnIncludeArchived"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Opportunity>> GetOpportunitiesAsync(
            String? strSearch = null,
            OpportunityStage? enmStage = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new opportunity to the system.
        /// </summary>
        /// <param name="objOpportunity"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Opportunity> AddOpportunityAsync(Opportunity objOpportunity, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing opportunity in the system.
        /// </summary>
        /// <param name="objOpportunity"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Opportunity?> UpdateOpportunityAsync(Opportunity objOpportunity, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Archives an opportunity, marking it as inactive or hidden in the system.
        /// </summary>
        /// <param name="objOpportunityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived opportunity, making it active or visible in the system again.
        /// </summary>
        /// <param name="objOpportunityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes an opportunity from the system.
        /// </summary>
        /// <param name="objOpportunityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteOpportunityAsync(Guid objOpportunityId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}