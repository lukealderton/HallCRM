namespace CRM.Core.Tickets.Domain
{
    /// <summary>
    /// Todo: replace with SearchResults<Ticket>
    /// </summary>
    public sealed class TicketSearchResult
    {
        public List<Ticket> Tickets { get; set; } = [];
        public Int32 TotalCount { get; set; }
    }
}