using CRM.Core.Contacts.Domain;

namespace CRM.Core.Contacts.Abstractions
{
    /// <summary>
    /// Allows management of contacts
    /// </summary>
    public interface IContactService
    {
        /// <summary>
        /// Gets a contact by its unique identifier.
        /// </summary>
        /// <param name="objContactId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Contact?> GetContactByIdAsync(Guid objContactId, CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of contacts based on search criteria.
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="objCompanyId"></param>
        /// <param name="blnIncludeArchived"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Contact>> GetContactsAsync(
            String? strSearch = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task<Contact> AddContactAsync(Contact objContact, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Archives a contact by its unique identifier.
        /// </summary>
        /// <param name="objContactId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived contact by its unique identifier.
        /// </summary>
        /// <param name="objContactId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes a contact by its unique identifier.
        /// </summary>
        /// <param name="objContactId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}