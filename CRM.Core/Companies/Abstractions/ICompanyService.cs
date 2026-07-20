using CRM.Core.Companies.Domain;

namespace CRM.Core.Companies.Abstractions
{
    public interface ICompanyService
    {
        /// <summary>
        /// Gets a company by its unique identifier.
        /// </summary>
        /// <param name="objCompanyId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Company?> GetCompanyByIdAsync(Guid objCompanyId, CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of companies based on search criteria.
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="blnIncludeArchived"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Company>> GetCompaniesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new company to the system.
        /// </summary>
        /// <param name="objCompany"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Company> AddCompanyAsync(Company objCompany, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing company in the system.
        /// </summary>
        /// <param name="objCompany"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Company?> UpdateCompanyAsync(Company objCompany, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Archives a company, marking it as inactive or hidden in the system.
        /// </summary>
        /// <param name="objCompanyId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived company, making it active or visible in the system again.
        /// </summary>
        /// <param name="objCompanyId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes a company from the system, marking it as permanently removed.
        /// </summary>
        /// <param name="objCompanyId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}