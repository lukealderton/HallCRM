namespace CRM.Core.Tickets.Domain
{
    public enum TicketStatus
    {
        New = 0,
        Triage = 1,
        InProgress = 2,
        WaitingOnClient = 3,
        WaitingOnInternal = 4,
        ReadyToInvoice = 5,
        Complete = 6,
        Cancelled = 7
    }
}