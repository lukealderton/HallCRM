using CRM.Core.Entities.Domain;

namespace CRM.Core.Entities.Abstractions
{
    public interface ICrmEntityService
    {
        /// <summary>
        /// Gets a CRM entity by its unique identifier.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<CrmEntity?> GetEntityByIdAsync(Guid objEntityId, CancellationToken objToken = default);

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
        /// Archives a CRM entity by its unique identifier.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores a previously archived CRM entity by its unique identifier.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes a CRM entity by its unique identifier.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteEntityAsync(Guid objEntityId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}