using CRM.Core.Companies.Abstractions;
using CRM.Core.Companies.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Companies.Services
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository objCompanyRepository)
        {
            _companyRepository = objCompanyRepository;
        }

        ///<inheritdoc/>
        public Task<Company?> GetCompanyByIdAsync(Guid objCompanyId, CancellationToken objToken = default)
        {
            return _companyRepository.GetCompanyByIdAsync(objCompanyId, false, objToken);
        }

        public Task<List<Company>> GetCompaniesAsync(
            String? strSearch = null,
            Boolean blnIncludeArchived = false,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            return _companyRepository.GetCompaniesAsync(
                strSearch,
                blnIncludeArchived,
                blnIncludeDeleted,
                objToken);
        }

        ///<inheritdoc/>
        public async Task<Company> AddCompanyAsync(Company objCompany, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (String.IsNullOrWhiteSpace(objCompany.Name))
            {
                throw new ArgumentException("Company name is required.", nameof(objCompany));
            }

            Guid objCompanyId = objCompany.Id == Guid.Empty
                ? Guid.NewGuid()
                : objCompany.Id;

            DateTime dteNow = DateTime.UtcNow;
            String strCompanyName = objCompany.Name.Trim();

            objCompany.Id = objCompanyId;
            objCompany.Name = strCompanyName;

            objCompany.Entity = new CrmEntity
            {
                Id = objCompanyId,
                EntityTypeId = (Int32)PredefinedEntityType.Company,
                DisplayName = strCompanyName,
                OwnerUserId = objUserId,
                CreatedUtc = dteNow,
                CreatedByUserId = objUserId
            };

            await _companyRepository.AddCompanyAsync(objCompany, objToken);
            await _companyRepository.SaveChangesAsync(objToken);

            return objCompany;
        }

        ///<inheritdoc/>
        public async Task<Company?> UpdateCompanyAsync(Company objCompany, Guid? objUserId = null, CancellationToken objToken = default)
        {
            if (objCompany.Id == Guid.Empty)
            {
                throw new ArgumentException("Company id is required.", nameof(objCompany));
            }

            if (String.IsNullOrWhiteSpace(objCompany.Name))
            {
                throw new ArgumentException("Company name is required.", nameof(objCompany));
            }

            Company? objExistingCompany = await _companyRepository.GetCompanyByIdAsync(objCompany.Id, true, objToken);

            if (objExistingCompany == null || objExistingCompany.Entity.DeletedUtc.HasValue)
            {
                return null;
            }

            DateTime dteNow = DateTime.UtcNow;
            String strCompanyName = objCompany.Name.Trim();

            objExistingCompany.Name = strCompanyName;
            objExistingCompany.TradingName = CleanString(objCompany.TradingName);
            objExistingCompany.Website = CleanString(objCompany.Website);
            objExistingCompany.PrimaryEmail = CleanString(objCompany.PrimaryEmail);
            objExistingCompany.PrimaryPhone = CleanString(objCompany.PrimaryPhone);

            objExistingCompany.AddressLine1 = CleanString(objCompany.AddressLine1);
            objExistingCompany.AddressLine2 = CleanString(objCompany.AddressLine2);
            objExistingCompany.Town = CleanString(objCompany.Town);
            objExistingCompany.County = CleanString(objCompany.County);
            objExistingCompany.Postcode = CleanString(objCompany.Postcode);
            objExistingCompany.Country = CleanString(objCompany.Country);

            objExistingCompany.CompanyNumber = CleanString(objCompany.CompanyNumber);
            objExistingCompany.CharityNumber = CleanString(objCompany.CharityNumber);
            objExistingCompany.VatNumber = CleanString(objCompany.VatNumber);
            objExistingCompany.Industry = CleanString(objCompany.Industry);
            objExistingCompany.Notes = CleanString(objCompany.Notes);

            objExistingCompany.Entity.DisplayName = strCompanyName;
            objExistingCompany.Entity.UpdatedUtc = dteNow;
            objExistingCompany.Entity.UpdatedByUserId = objUserId;

            await _companyRepository.SaveChangesAsync(objToken);

            return objExistingCompany;
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Company? objCompany = await _companyRepository.GetCompanyByIdAsync(objCompanyId, true, objToken);

            if (objCompany == null || objCompany.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objCompany.Entity.ArchivedUtc = dteNow;
            objCompany.Entity.ArchivedByUserId = objUserId;
            objCompany.Entity.UpdatedUtc = dteNow;
            objCompany.Entity.UpdatedByUserId = objUserId;

            await _companyRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Company? objCompany = await _companyRepository.GetCompanyByIdAsync(objCompanyId, true, objToken);

            if (objCompany == null || objCompany.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objCompany.Entity.ArchivedUtc = null;
            objCompany.Entity.ArchivedByUserId = null;
            objCompany.Entity.UpdatedUtc = dteNow;
            objCompany.Entity.UpdatedByUserId = objUserId;

            await _companyRepository.SaveChangesAsync(objToken);

            return true;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteCompanyAsync(Guid objCompanyId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            Company? objCompany = await _companyRepository.GetCompanyByIdAsync(objCompanyId, true, objToken);

            if (objCompany == null || objCompany.Entity.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dteNow = DateTime.UtcNow;

            objCompany.Entity.DeletedUtc = dteNow;
            objCompany.Entity.DeletedByUserId = objUserId;
            objCompany.Entity.UpdatedUtc = dteNow;
            objCompany.Entity.UpdatedByUserId = objUserId;

            await _companyRepository.SaveChangesAsync(objToken);

            return true;
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