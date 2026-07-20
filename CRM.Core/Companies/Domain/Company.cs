using CRM.Core.Entities.Domain;

namespace CRM.Core.Companies.Domain
{
    public sealed class Company : CrmEntityRecord
    {
        public String Name { get; set; } = String.Empty;
        public String? TradingName { get; set; }

        public String? Website { get; set; }

        public String? PrimaryEmail { get; set; }
        public String? PrimaryPhone { get; set; }

        public String? AddressLine1 { get; set; }
        public String? AddressLine2 { get; set; }
        public String? Town { get; set; }
        public String? County { get; set; }
        public String? Postcode { get; set; }
        public String? Country { get; set; }

        public String? CompanyNumber { get; set; }
        public String? CharityNumber { get; set; }
        public String? VatNumber { get; set; }

        public String? Industry { get; set; }

        public String? Notes { get; set; }
    }
}