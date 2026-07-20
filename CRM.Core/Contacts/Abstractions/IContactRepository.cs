using CRM.Core.Contacts.Domain;

namespace CRM.Core.Contacts.Abstractions
{
    public interface IContactRepository
    {
        Task<Contact?> GetContactByIdAsync(Guid objContactId, Boolean blnTracking = false, CancellationToken objToken = default);

        Task<List<Contact>> GetContactsAsync(
            String? strSearch = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        Task AddContactAsync(Contact objContact, CancellationToken objToken = default);

        Task SaveChangesAsync(CancellationToken objToken = default);
    }
}