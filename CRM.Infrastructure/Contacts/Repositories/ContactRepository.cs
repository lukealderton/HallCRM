using CRM.Core.Contacts.Abstractions;
using CRM.Core.Contacts.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Contacts.Repositories
{
    public sealed class ContactRepository : IContactRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public ContactRepository(IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory = objDbContextFactory;
        }

        public async Task<Contact?> GetContactByIdAsync(Guid objContactId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Contact> objQuery = objDbContext.Contacts
                .Include(objContact => objContact.Entity)
                .Include(objContact => objContact.Company)
                    .ThenInclude(objCompany => objCompany!.Entity);

            if (!blnTracking)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return await objQuery.FirstOrDefaultAsync(objContact => objContact.Id == objContactId, objToken);
        }

        public async Task<List<Contact>> GetContactsAsync(
            String? strSearch = null,
            Guid? objCompanyId = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Contact> objQuery = objDbContext.Contacts
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

            return await objQuery
                .OrderBy(objContact => objContact.LastName)
                .ThenBy(objContact => objContact.FirstName)
                .ThenBy(objContact => objContact.PrimaryEmail)
                .ToListAsync(objToken);
        }

        public async Task AddContactAsync(Contact objContact, CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);
            await objDbContext.Contacts.AddAsync(objContact, objToken);
        }

        public async Task SaveChangesAsync(CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext = await _objDbContextFactory.CreateDbContextAsync(objToken);
            await objDbContext.SaveChangesAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountContactsAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Contacts
                .AsNoTracking()
                .Where(objContact =>
                    !objContact.Entity.ArchivedUtc.HasValue &&
                    !objContact.Entity.DeletedUtc.HasValue)
                .CountAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Int32> CountContactableContactsAsync(
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Contacts
                .AsNoTracking()
                .Where(objContact =>
                    !objContact.Entity.ArchivedUtc.HasValue &&
                    !objContact.Entity.DeletedUtc.HasValue &&
                    (
                        objContact.PrimaryEmail != null ||
                        objContact.PrimaryPhone != null ||
                        objContact.MobilePhone != null
                    ))
                .CountAsync(objToken);
        }
    }
}