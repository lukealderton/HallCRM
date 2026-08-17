using CRM.Core.Companies.Domain;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Jobs.Domain
{
    public sealed class Job : CrmEntityRecord
    {
        public Guid? CompanyId { get; set; }
        public Guid? ContactId { get; set; }

        public Guid? AssignedUserId { get; set; }

        public String Name { get; set; } =
            String.Empty;

        public String? Description { get; set; }

        public JobStage Stage { get; set; } =
            JobStage.New;

        public Decimal? Value { get; set; }

        public Int32? ProbabilityPercent { get; set; }

        public DateTime? ExpectedCloseDateUtc { get; set; }

        public String? Source { get; set; }

        /*
         * Job site snapshot.
         *
         * These belong to the Job rather than a separate
         * Property entity so the historical address remains
         * exactly as it was when the work was carried out.
         */

        public String? AddressLine1 { get; set; }

        public String? AddressLine2 { get; set; }

        public String? Town { get; set; }

        public String? County { get; set; }

        public String? Postcode { get; set; }

        public String? SiteContactName { get; set; }

        public String? SiteContactPhone { get; set; }

        public String? AccessNotes { get; set; }

        public String? Notes { get; set; }

        public Company? Company { get; set; }

        public Contact? Contact { get; set; }

        public ICollection<JobServiceLink> ServiceLinks { get; set; } = [];
    }
}