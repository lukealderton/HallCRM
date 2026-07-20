namespace CRM.Core.Tickets.Domain
{
    public sealed class TicketComment
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public Guid? CreatedByUserId { get; set; }
        public DateTime CreatedUtc { get; set; }

        public Boolean IsInternal { get; set; } = true;

        public String Text { get; set; } = String.Empty;
    }
}