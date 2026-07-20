using CRM.Core.Tickets.Domain;

namespace CRM.Core.Tickets.Abstractions
{
    public interface ITicketService
    {
        /// <summary>
        /// Gets a ticket by its unique identifier.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Ticket?> GetTicketByIdAsync(Guid objTicketId, CancellationToken objToken = default);

        /// <summary>
        /// Gets a list of tickets based on the provided search request.
        /// </summary>
        /// <param name="objRequest"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<TicketSearchResult> GetTicketsAsync(TicketSearchRequest objRequest, CancellationToken objToken = default);

        /// <summary>
        /// Adds a new ticket to the system.
        /// </summary>
        /// <param name="objTicket"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Ticket?> AddTicketAsync(Ticket objTicket, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing ticket in the system.
        /// </summary>
        /// <param name="objTicket"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Ticket?> UpdateTicketAsync(Ticket objTicket, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Sets the status of a ticket.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="enmStatus"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> SetStatusAsync(Guid objTicketId, TicketStatus enmStatus, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Marks a ticket as invoiced.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> MarkInvoicedAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Adds a comment to a ticket.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="strComment"></param>
        /// <param name="blnIsInternal"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> AddCommentAsync(Guid objTicketId, String strComment, Boolean blnIsInternal = true, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Adds a time entry to a ticket.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="intMinutes"></param>
        /// <param name="strNotes"></param>
        /// <param name="blnIsChargeable"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> AddTimeEntryAsync(Guid objTicketId, Int32 intMinutes, String? strNotes = null, Boolean blnIsChargeable = true, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Archives a ticket, marking it as inactive or no longer relevant.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> ArchiveTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Restores an archived ticket, making it active again.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> RestoreTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default);

        /// <summary>
        /// Deletes a ticket from the system.
        /// </summary>
        /// <param name="objTicketId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default);
    }
}