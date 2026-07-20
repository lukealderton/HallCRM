using CRM.Core.Companies.Domain;
using CRM.Core.Entities.Domain;

namespace CRM.Core.Tickets.Domain
{
    public sealed class Ticket : CrmEntityRecord
    {
        public String Title { get; set; } = String.Empty;
        public String? Description { get; set; }

        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }

        public Guid? ContactId { get; set; }
        public Guid? OpportunityId { get; set; }
        public Guid? ProjectId { get; set; }

        public Guid? AssignedToUserId { get; set; }

        public TicketType Type { get; set; } = TicketType.General;
        public TicketStatus Status { get; set; } = TicketStatus.New;
        public TicketPriority Priority { get; set; } = TicketPriority.Normal;

        public Boolean IsChargeable { get; set; } = true;
        public Boolean IsInvoiced { get; set; }

        public Decimal? EstimatedValue { get; set; }
        public Decimal? QuotedValue { get; set; }
        public Decimal? InvoiceValue { get; set; }

        public Int32? EstimatedMinutes { get; set; }
        public Int32 ActualMinutes { get; set; }

        public DateTime? DueUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
        public Guid? CompletedByUserId { get; set; }

        public String? InternalNotes { get; set; }

        public ICollection<TicketComment> Comments { get; set; } = [];
        public ICollection<TicketTimeEntry> TimeEntries { get; set; } = [];
        public ICollection<TicketStatusHistory> StatusHistory { get; set; } = [];
    }
}