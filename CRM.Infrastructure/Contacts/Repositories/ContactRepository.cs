using CRM.Core.Contacts.Abstractions;
using CRM.Core.Contacts.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Contacts.Repositories
{
    public sealed class ContactRepository : IContactRepository
    {
        private readonly CRMDbContext _context;

        public ContactRepository(CRMDbContext objContext)
        {
            _context = objContext;
        }

        public Task<Contact?> GetContactByIdAsync(Guid objContactId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            IQueryable<Contact> objQuery = _context.Contacts
                .Include(objContact => objContact.Entity)
                .Include(objContact => objContact.Company)
                    .ThenInclude(objCompany => objCompany!.Entity);

            if (!blnTracking)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return objQuery.FirstOrDefaultAsync(objContact => objContact.Id == objContactId, objToken);
        }

        public Task<List<Contact>> GetContactsAsync(
            String? strSearch = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<Contact> objQuery = _context.Contacts
                .AsNoTracking()
                .Include(objContact => objContact.Entity)
                .Include(objContact => objContact.Company)
                    .ThenInclude(objCompany => objCompany!.Entity);

            if (!blnIncludeDeleted)
            {
                objQuery = objQuery.Where(objContact => !objContact.Entity.DeletedUtc.HasValue);
            }

            if (!blnIncludeArchived)
            {
                objQuery = objQuery.Where(objContact => !objContact.Entity.ArchivedUtc.HasValue);
            }

            if (objCompanyId.HasValue)
            {
                objQuery = objQuery.Where(objContact => objContact.CompanyId == objCompanyId.Value);
            }

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strKeyword = strSearch.Trim();

                objQuery = objQuery.Where(objContact =>
                    (objContact.FirstName != null && objContact.FirstName.Contains(strKeyword)) ||
                    (objContact.LastName != null && objContact.LastName.Contains(strKeyword)) ||
                    (objContact.PrimaryEmail != null && objContact.PrimaryEmail.Contains(strKeyword)) ||
                    (objContact.PrimaryPhone != null && objContact.PrimaryPhone.Contains(strKeyword)) ||
                    (objContact.MobilePhone != null && objContact.MobilePhone.Contains(strKeyword)) ||
                    (objContact.Company != null && objContact.Company.Name.Contains(strKeyword)));
            }

            return objQuery
                .OrderBy(objContact => objContact.LastName)
                .ThenBy(objContact => objContact.FirstName)
                .ThenBy(objContact => objContact.PrimaryEmail)
                .ToListAsync(objToken);
        }

        public Task AddContactAsync(Contact objContact, CancellationToken objToken = default)
        {
            return _context.Contacts.AddAsync(objContact, objToken).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken objToken = default)
        {
            return _context.SaveChangesAsync(objToken);
        }
    }
}