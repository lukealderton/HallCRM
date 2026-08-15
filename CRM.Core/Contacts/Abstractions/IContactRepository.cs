using CRM.Core.Contacts.Domain;

namespace CRM.Core.Contacts.Abstractions
{
    public interface IContactRepository
    {
        /// <summary>
        /// Gets a specific contact
        /// </summary>
        /// <param name="objContactId"></param>
        /// <param name="blnTracking"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Contact?> GetContactByIdAsync(Guid objContactId, CancellationToken objToken = default);

        /// <summary>
        /// Gets/searches contacts
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

        /// <summary>
        /// Adds a new contact to the repository
        /// </summary>
        /// <param name="objContact"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddContactAsync(
            Contact objContact,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing contact in the repository
        /// </summary>
        /// <param name="objContact"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task UpdateContactAsync(
            Contact objContact,
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of contacts in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountContactsAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of contactable contacts in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountContactableContactsAsync(
            CancellationToken objToken = default);
    }
}