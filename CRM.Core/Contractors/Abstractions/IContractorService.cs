using CRM.Core.Contractors.Domain;

namespace CRM.Core.Contractors.Abstractions;

public interface IContractorService
{
    Task<Contractor?> GetContractorAsync(Guid objContractorId, CancellationToken objToken = default);
    Task<List<Contractor>> GetContractorsAsync(Boolean blnIncludeInactive = false, String? strSearch = null, CancellationToken objToken = default);
    Task<Contractor> SaveContractorAsync(Contractor objContractor, CancellationToken objToken = default);
}
