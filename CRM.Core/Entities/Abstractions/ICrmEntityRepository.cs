using CRM.Core.Entities.Domain;

namespace CRM.Core.Entities.Abstractions
{
    public interface ICrmEntityRepository
    {
        /// <summary>
        /// Gets a CRM entity by its unique identifier.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<CrmEntity?> GetEntityByIdAsync(
            Guid objEntityId,
            CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of CRM entities based on the specified criteria.
        /// </summary>
        /// <param name="intEntityTypeId"></param>
        /// <param name="strSearch"></param>
        /// <param name="blnIncludeArchived"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<CrmEntity>> GetEntitiesAsync(
            Int32? intEntityTypeId = null,
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Checks whether a CRM entity exists and has not been deleted.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> EntityExistsAsync(
            Guid objEntityId,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new CRM entity.
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddEntityAsync(
            CrmEntity objEntity,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing CRM entity.
        /// </summary>
        /// <param name="objEntity"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task UpdateEntityAsync(
            CrmEntity objEntity,
            CancellationToken objToken = default);
    }
}