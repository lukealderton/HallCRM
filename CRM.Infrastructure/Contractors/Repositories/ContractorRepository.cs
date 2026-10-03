using CRM.Core.Contractors.Abstractions;
using CRM.Core.Contractors.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Contractors.Repositories;

public sealed class ContractorRepository(IDbContextFactory<CRMDbContext> objFactory) : IContractorRepository
{
    public async Task<Contractor?> GetContractorAsync(Guid objContractorId, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        return await objContext.Contractors.AsNoTracking().FirstOrDefaultAsync(objContractor => objContractor.Id == objContractorId, objToken);
    }

    public async Task<List<Contractor>> GetContractorsAsync(Boolean blnIncludeInactive = false, String? strSearch = null, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        IQueryable<Contractor> objQuery = objContext.Contractors.AsNoTracking();
        if (!blnIncludeInactive) objQuery = objQuery.Where(objContractor => objContractor.Enabled);
        if (!String.IsNullOrWhiteSpace(strSearch))
        {
            objQuery = objQuery.Where(objContractor => objContractor.Name.Contains(strSearch) ||
                (objContractor.CompanyName != null && objContractor.CompanyName.Contains(strSearch)) ||
                (objContractor.Email != null && objContractor.Email.Contains(strSearch)) ||
                (objContractor.Phone != null && objContractor.Phone.Contains(strSearch)));
        }
        return await objQuery.OrderBy(objContractor => objContractor.Name).ToListAsync(objToken);
    }

    public async Task SaveAsync(Contractor objContractor, Boolean blnNew, CancellationToken objToken = default)
    {
        await using CRMDbContext objContext = await objFactory.CreateDbContextAsync(objToken);
        if (blnNew) objContext.Contractors.Add(objContractor);
        else objContext.Contractors.Update(objContractor);
        await objContext.SaveChangesAsync(objToken);
    }
}
