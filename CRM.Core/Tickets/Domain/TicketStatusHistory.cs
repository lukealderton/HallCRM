namespace CRM.Core.Tickets.Domain
{
    public sealed class TicketStatusHistory
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public TicketStatus? PreviousStatus { get; set; }
        public TicketStatus NewStatus { get; set; }

        public Guid? ChangedByUserId { get; set; }
        public DateTime ChangedUtc { get; set; }
    }
}