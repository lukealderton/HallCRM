using CRM.Core.Contacts.Abstractions;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Contacts.Services
{
    /// <summary>
    /// Represents a service for managing contacts in the CRM system.
    /// </summary>
    public sealed class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;

        public ContactService(IContactRepository objContactRepository)
        {
            _contactRepository = objContactRepository;
        }

        ///<inheritdoc/>
        public Task<Contact?> GetContactByIdAsync(Guid objContactId, CancellationToken objToken = default)
        {
            return _contactRepository.GetContactByIdAsync(objContactId, false, objToken);
        }

        ///<inheritdoc/>
        public Task<List<Contact>> GetContactsAsync(
            String? strSearch = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _contactRepository.GetContactsAsync(
                strSearch,
                objCompanyId,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Contact> AddContactAsync(Contact objContact, Guid? objUserId = null, CancellationToken objToken = default)
        {
            String strDisplayName = BuildDisplayName(objContact);

            if (String.IsNullOrWhiteSpace(strDisplayName))
            {
                throw new ArgumentException("Contact name or email is required.", nameof(objContact));
            }

            Guid objContactId = objContact.Id == Guid.Empty
                ? Guid.NewGuid()
                : objContact.Id;

            DateTime dteNow = DateTime.UtcNow;

            objContact.Id = objContactId;
            objContact.Title = CleanString(objContact.Title);
            objContact.FirstName = CleanString(objContact.FirstName);
            objContact.LastName = CleanString(objContact.LastName);
            objContact.JobTitle = CleanString(objContact.JobTitle);
            objContact.Department = CleanString(objContact.Department);
            objContact.PrimaryEmail = CleanString(objContact.PrimaryEmail);
            objContact.PrimaryPhone = CleanString(objContact.PrimaryPhone);
            objContact.MobilePhone = CleanString(objContact.MobilePhone);
            objContact.LinkedInUrl = CleanString(objContact.LinkedInUrl);
            objContact.Notes = CleanString(objContact.Notes);

            objContact.Entity = new CrmEntity
            {
                Id = objContactId,
                EntityTypeId = (Int32)PredefinedEntityType.Contact,
                DisplayName = strDisplayName,
                OwnerUserId = objUserId,
                CreatedUtc = dteNow,
                CreatedByUserId = objUserId
            };

            await _contactRepository.AddContactAsync(objContact, objToken);
            await _contactRepository.SaveChangesAsync(objToken);

            return objContact;
        }

        ///<inheritdoc/>
        public async Task<Contact?> UpdateContactAsync(Contact objContact, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (objContact.Id == Guid.Empty)
            {
                throw new ArgumentException("Contact id is required.", nameof(objContact));
            }

            String strDisplayName = BuildDisplayName(objContact);

            if (String.IsNullOrWhiteSpace(strDisplayName))
            {
                throw new ArgumentException("Contact name or email is required.", nameof(objContact));
            }

            Contact? objExistingContact = await _contactRepository.GetContactByIdAsync(objContact.Id, true, objToken);

            if (objExistingContact == null || objExistingContact.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow = DateTime.UtcNow;

            objExistingContact.CompanyId = objContact.CompanyId;
            objExistingContact.Title = CleanString(objContact.Title);
            objExistingContact.FirstName = CleanString(objContact.FirstName);
            objExistingContact.LastName = CleanString(objContact.LastName);
            objExistingContact.JobTitle = CleanString(objContact.JobTitle);
            objExistingContact.Department = CleanString(objContact.Department);
            objExistingContact.PrimaryEmail = CleanString(objContact.PrimaryEmail);
            objExistingContact.PrimaryPhone = CleanString(objContact.PrimaryPhone);
            objExistingContact.MobilePhone = CleanString(objContact.MobilePhone);
            objExistingContact.LinkedInUrl = CleanString(objContact.LinkedInUrl);
            objExistingContact.Notes = CleanString(objContact.Notes);

            objExistingContact.Entity.DisplayName = strDisplayName;
            objExistingContact.Entity.UpdatedUtc = dteNow;
            objExistingContact.Entity.UpdatedByUserId = objUserId;

            await _contactRepository.SaveChangesAsync(objToken);

            return objExistingContact;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Contact? objContact = await _contactRepository.GetContactByIdAsync(objContactId, true, objToken);

            if (objContact == null || objContact.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objContact.Entity.ArchivedUtc = dteNow;
            objContact.Entity.ArchivedByUserId = objUserId;
            objContact.Entity.UpdatedUtc = dteNow;
            objContact.Entity.UpdatedByUserId = objUserId;

            await _contactRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Contact? objContact = await _contactRepository.GetContactByIdAsync(objContactId, true, objToken);

            if (objContact == null || objContact.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objContact.Entity.ArchivedUtc = null;
            objContact.Entity.ArchivedByUserId = null;
            objContact.Entity.UpdatedUtc = dteNow;
            objContact.Entity.UpdatedByUserId = objUserId;

            await _contactRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteContactAsync(Guid objContactId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Contact? objContact = await _contactRepository.GetContactByIdAsync(objContactId, true, objToken);

            if (objContact == null || objContact.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objContact.Entity.DeletedUtc = dteNow;
            objContact.Entity.DeletedByUserId = objUserId;
            objContact.Entity.UpdatedUtc = dteNow;
            objContact.Entity.UpdatedByUserId = objUserId;

            await _contactRepository.SaveChangesAsync(objToken);

            return true;
        }

        /// <summary>
        /// Builds a display name for a contact based on their first and last names.
        /// </summary>
        /// <param name="objContact"></param>
        /// <returns></returns>
        private static String BuildDisplayName(Contact objContact)
        {
            String strFullName = $"{objContact.FirstName} {objContact.LastName}".Trim();

            if (!String.IsNullOrWhiteSpace(strFullName))
            {
                return strFullName;
            }

            if (!String.IsNullOrWhiteSpace(objContact.PrimaryEmail))
            {
                return objContact.PrimaryEmail.Trim();
            }

            if (!String.IsNullOrWhiteSpace(objContact.PrimaryPhone))
            {
                return objContact.PrimaryPhone.Trim();
            }

            if (!String.IsNullOrWhiteSpace(objContact.MobilePhone))
            {
                return objContact.MobilePhone.Trim();
            }

            return String.Empty;
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
    }
}