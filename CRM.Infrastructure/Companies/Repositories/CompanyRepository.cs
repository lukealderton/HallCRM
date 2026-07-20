using CRM.Core.Companies.Abstractions;
using CRM.Core.Companies.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Companies.Repositories
{
    public sealed class CompanyRepository : ICompanyRepository
    {
        private readonly CRMDbContext _context;

        public CompanyRepository(CRMDbContext objContext)
        {
            _context = objContext;
        }

        public async Task<Company?> GetCompanyByIdAsync(Guid objCompanyId, Boolean blnAsTracking = false, CancellationToken objToken = default)
        {
            IQueryable<Company> colQuery = _context.Companies
                .Include(x => x.Entity)
                .ThenInclude(x => x.EntityType);

            if (!blnAsTracking)
            {
                colQuery = colQuery.AsNoTracking();
            }

            return await colQuery.FirstOrDefaultAsync(x => x.Id == objCompanyId, objToken);
        }

        public async Task<List<Company>> GetCompaniesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<Company> colQuery = _context.Companies
                .Include(x => x.Entity)
                .AsNoTracking();

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strSearchTerm = strSearch.Trim();

                colQuery = colQuery.Where(x =>
                    x.Name.Contains(strSearchTerm) ||
                    (x.TradingName != null && x.TradingName.Contains(strSearchTerm)) ||
                    (x.PrimaryEmail != null && x.PrimaryEmail.Contains(strSearchTerm)) ||
                    (x.PrimaryPhone != null && x.PrimaryPhone.Contains(strSearchTerm)) ||
                    (x.CompanyNumber != null && x.CompanyNumber.Contains(strSearchTerm)));
            }

            if (!blnIncludeArchived)
            {
                colQuery = colQuery.Where(x => !x.Entity.ArchivedUtc.HasValue);
            }

            if (!blnIncludeDeleted)
            {
                colQuery = colQuery.Where(x => !x.Entity.DeletedUtc.HasValue);
            }

            return await colQuery
                .OrderBy(x => x.Name)
                .ToListAsync(objToken);
        }

        public async Task AddCompanyAsync(Company objCompany, CancellationToken objToken = default)
        {
            await _context.Companies.AddAsync(objCompany, objToken);
        }

        public async Task SaveChangesAsync(CancellationToken objToken = default)
        {
            await _context.SaveChangesAsync(objToken);
        }
    }
}