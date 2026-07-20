using CRM.Core.Companies.Domain;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Jobs.Domain
{
    public sealed class Job : CrmEntityRecord
    {
        public Guid? CompanyId { get; set; }
        public Guid? ContactId { get; set; }

        public String Name { get; set; } = String.Empty;
        public String? Description { get; set; }

        public JobStage Stage { get; set; } = JobStage.New;

        public Decimal? Value { get; set; }
        public Int32? ProbabilityPercent { get; set; }

        public DateTime? ExpectedCloseDateUtc { get; set; }

        public String? Source { get; set; }
        public String? Notes { get; set; }

        public Company? Company { get; set; }
        public Contact? Contact { get; set; }
    }
}