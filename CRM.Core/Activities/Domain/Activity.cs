using CRM.Core.Companies.Domain;
using CRM.Core.Contacts.Domain;
using CRM.Core.Entities.Domain;
using CRM.Core.Jobs.Domain;

namespace CRM.Core.Activities.Domain
{
    public sealed class Activity : CrmEntityRecord
    {
        public Guid? CompanyId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? JobId { get; set; }

        public Guid? AssignedUserId { get; set; }

        public ActivityType Type { get; set; } =
            ActivityType.Task;

        public String Subject { get; set; } =
            String.Empty;

        public String? Description { get; set; }

        public DateTime? DueUtc { get; set; }

        public DateTime? CompletedUtc { get; set; }

        public Company? Company { get; set; }
        public Contact? Contact { get; set; }
        public Job? Job { get; set; }

        public Boolean IsCompleted =>
            CompletedUtc.HasValue;

        public Boolean IsOpen =>
            !CompletedUtc.HasValue &&
            IsActive;
    }
}