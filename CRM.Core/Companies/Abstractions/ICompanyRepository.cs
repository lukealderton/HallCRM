using CRM.Core.Companies.Domain;

namespace CRM.Core.Companies.Abstractions
{
    public interface ICompanyRepository
    {
        /// <summary>
        /// Gets a company by its unique identifier.
        /// </summary>
        /// <param name="objCompanyId"></param>
        /// <param name="blnAsTracking"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Company?> GetCompanyByIdAsync(Guid objCompanyId, Boolean blnAsTracking = false, CancellationToken objToken = default);

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
        /// Adds a new company to the repository.
        /// </summary>
        /// <param name="objCompany"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddCompanyAsync(Company objCompany, CancellationToken objToken = default);

        /// <summary>
        /// Saves changes made to the repository, persisting them to the underlying data store.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken objToken = default);

        /// <summary>
        /// Counts the total number of companies in the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Int32> CountCompaniesAsync(
            CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of the most recently added companies, limited by the specified number to take.
        /// </summary>
        /// <param name="intTake"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Company>> GetRecentCompaniesAsync(
            Int32 intTake,
            CancellationToken objToken = default);
    }
}