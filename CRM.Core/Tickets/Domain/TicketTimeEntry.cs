namespace CRM.Core.Tickets.Domain
{
    public sealed class TicketTimeEntry
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public Guid? UserId { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime? WorkDateUtc { get; set; }

        public Int32 Minutes { get; set; }

        public Boolean IsChargeable { get; set; } = true;
        public Boolean IsInvoiced { get; set; }

        public String? Notes { get; set; }
    }
}