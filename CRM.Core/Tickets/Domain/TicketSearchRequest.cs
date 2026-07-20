namespace CRM.Core.Tickets.Domain
{
    public sealed class TicketSearchRequest
    {
        public String? Search { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? AssignedToUserId { get; set; }

        public TicketStatus? Status { get; set; }
        public TicketType? Type { get; set; }
        public TicketPriority? Priority { get; set; }

        public Boolean? IsChargeable { get; set; }
        public Boolean? IsInvoiced { get; set; }

        public Boolean HideInvoiced { get; set; }
        public Boolean IncludeArchived { get; set; }
        public Boolean IncludeDeleted { get; set; }

        public Int32 PageSize { get; set; } = 50;
        public Int32 Skip { get; set; }
    }
}