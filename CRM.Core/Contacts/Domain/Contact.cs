using CRM.Core.Companies.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Contacts.Domain
{
    public sealed class Contact : CrmEntityRecord
    {
        public Guid? CompanyId { get; set; }

        public String? Title { get; set; }
        public String? FirstName { get; set; }
        public String? LastName { get; set; }

        public String? JobTitle { get; set; }
        public String? Department { get; set; }

        public String? PrimaryEmail { get; set; }
        public String? PrimaryPhone { get; set; }
        public String? MobilePhone { get; set; }

        public String? LinkedInUrl { get; set; }

        public String? Notes { get; set; }

        public Company? Company { get; set; }
    }
}