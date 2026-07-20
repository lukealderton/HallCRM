using CRM.Core.Tickets.Domain;

namespace CRM.Core.Tickets.Abstractions
{
    public interface ITicketRepository
    {
        /// <summary>
        /// Gets a ticket by its unique identifier.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="blnTrackChanges"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Ticket?> GetTicketByIdAsync(Guid objTicketId, Boolean blnTrackChanges = false, CancellationToken objToken = default);

        /// <summary>
        /// Gets tickets based on the specified search request.
        /// </summary>
        /// <param name="objRequest"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<TicketSearchResult> GetTicketsAsync(TicketSearchRequest objRequest, CancellationToken objToken = default);

        /// <summary>
        /// Adds a new ticket to the repository.
        /// </summary>
        /// <param name="objTicket"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddTicketAsync(Ticket objTicket, CancellationToken objToken = default);

        /// <summary>
        /// Adds a new comment to a ticket in the repository.
        /// </summary>
        /// <param name="objComment"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddTicketCommentAsync(TicketComment objComment, CancellationToken objToken = default);

        /// <summary>
        /// Adds a new time entry to a ticket in the repository.
        /// </summary>
        /// <param name="objTimeEntry"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddTicketTimeEntryAsync(TicketTimeEntry objTimeEntry, CancellationToken objToken = default);

        /// <summary>
        /// Adds a new status history entry to a ticket in the repository.
        /// </summary>
        /// <param name="objStatusHistory"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddTicketStatusHistoryAsync(TicketStatusHistory objStatusHistory, CancellationToken objToken = default);

        /// <summary>
        /// Saves changes made to the repository, persisting them to the underlying data store.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken objToken = default);
    }
}