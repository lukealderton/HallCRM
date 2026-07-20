using CRM.Core.Opportunities.Domain;

namespace CRM.Core.Opportunities.Abstractions
{
    public interface IOpportunityRepository
    {
        /// <summary>
        /// Gets an opportunity by its unique identifier.
        /// </summary>
        /// <param name="objOpportunityId"></param>
        /// <param name="blnTracking"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Opportunity?> GetOpportunityByIdAsync(Guid objOpportunityId, Boolean blnTracking = false, CancellationToken objToken = default);

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
        /// Adds a new opportunity to the repository.
        /// </summary>
        /// <param name="objOpportunity"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddOpportunityAsync(Opportunity objOpportunity, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing opportunity in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken objToken = default);
    }
}