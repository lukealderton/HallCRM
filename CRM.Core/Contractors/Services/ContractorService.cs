using System.ComponentModel.DataAnnotations;
using CRM.Core.Contractors.Abstractions;
using CRM.Core.Contractors.Domain;

namespace CRM.Core.Contractors.Services;

public sealed class ContractorService(IContractorRepository objRepository) : IContractorService
{
    public Task<Contractor?> GetContractorAsync(Guid objContractorId, CancellationToken objToken = default) =>
        objRepository.GetContractorAsync(objContractorId, objToken);

    public Task<List<Contractor>> GetContractorsAsync(Boolean blnIncludeInactive = false, String? strSearch = null, CancellationToken objToken = default) =>
        objRepository.GetContractorsAsync(blnIncludeInactive, strSearch?.Trim(), objToken);

    public async Task<Contractor> SaveContractorAsync(Contractor objContractor, CancellationToken objToken = default)
    {
        objContractor.Name = objContractor.Name?.Trim() ?? String.Empty;
        objContractor.CompanyName = CleanString(objContractor.CompanyName);
        objContractor.Email = CleanString(objContractor.Email);
        objContractor.Phone = CleanString(objContractor.Phone);
        objContractor.Notes = CleanString(objContractor.Notes);
        Validator.ValidateObject(objContractor, new ValidationContext(objContractor), true);

        Boolean blnNew = objContractor.Id == Guid.Empty;
        if (blnNew)
        {
            objContractor.Id = Guid.NewGuid();
            objContractor.CreatedUtc = DateTime.UtcNow;
        }
        else
        {
            Contractor objExisting = await objRepository.GetContractorAsync(objContractor.Id, objToken)
                ?? throw new KeyNotFoundException("Contractor not found.");
            objContractor.CreatedUtc = objExisting.CreatedUtc;
            objContractor.UpdatedUtc = DateTime.UtcNow;
        }

        try
        {
            await objRepository.SaveAsync(objContractor, blnNew, objToken);
        }
        catch
        {
            // Keep a failed new form retryable rather than treating it as an existing record.
            if (blnNew) objContractor.Id = Guid.Empty;
            throw;
        }
        return objContractor;
    }

    private static String? CleanString(String? strValue) => String.IsNullOrWhiteSpace(strValue) ? null : strValue.Trim();
}
