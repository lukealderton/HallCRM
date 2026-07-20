using CRM.Core.Entities.Domain;
using CRM.Core.Logging.Abstraction;
using CRM.Core.Tickets.Abstractions;
using CRM.Core.Tickets.Domain;
using CRM.Primitives.Common.Enums;
using CRM.Primitives.Logging.Enums;

namespace CRM.Core.Tickets.Services
{
    public sealed class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogService _logService;

        public TicketService(
            ITicketRepository objTicketRepository,
            ILogService objLogService)
        {
            _ticketRepository = objTicketRepository;
            _logService = objLogService;
        }

        ///<inheritdoc/>
        public async Task<Ticket?> GetTicketByIdAsync(Guid objTicketId, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return null;
                }

                return await _ticketRepository.GetTicketByIdAsync(objTicketId, false, objToken);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                        objError,
                        LogArea.Tickets,
                        null,
                        objTicketId,
                        ItemType.None,
                        "Failed to get ticket by id.",
                        objToken);

                return null;
            }
        }

        ///<inheritdoc/>
        public async Task<TicketSearchResult> GetTicketsAsync(TicketSearchRequest objRequest, CancellationToken objToken = default)
        {
            try
            {
                if (objToken.IsCancellationRequested)
                {
                    return new TicketSearchResult();
                }

                objRequest ??= new TicketSearchRequest();

                return await _ticketRepository.GetTicketsAsync(objRequest, objToken);
            }
            catch (OperationCanceledException)
            {
                return new TicketSearchResult();
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                        objError,
                        LogArea.Tickets,
                        null,
                        null,
                        ItemType.None,
                        "Failed to get tickets.",
                        objToken);

                return new TicketSearchResult();
            }
        }

        ///<inheritdoc/>
        public async Task<Ticket?> AddTicketAsync(Ticket objTicket, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objToken.IsCancellationRequested)
                {
                    return null;
                }

                if (objTicket == null)
                {
                    await _logService.LogErrorAsync(
                        new ArgumentNullException(nameof(objTicket)),
                        LogArea.Tickets,
                        objUserId,
                        null,
                        ItemType.None,
                        "Ticket could not be added because the model was null.",
                        objToken);

                    return null;
                }

                if (String.IsNullOrWhiteSpace(objTicket.Title))
                {
                    await _logService.LogErrorAsync(
                        new ArgumentException("Ticket title is required.", nameof(objTicket)),
                        LogArea.Tickets,
                        objUserId,
                        objTicket.Id == Guid.Empty ? null : objTicket.Id,
                        ItemType.None,
                        "Ticket could not be added because the title was empty.",
                        objToken);

                    return null;
                }

                Guid objTicketId = objTicket.Id == Guid.Empty
                    ? Guid.NewGuid()
                    : objTicket.Id;

                DateTime dteNow = DateTime.UtcNow;
                String strTitle = objTicket.Title.Trim();

                objTicket.Id = objTicketId;
                objTicket.Title = strTitle;
                objTicket.Description = CleanString(objTicket.Description);
                objTicket.InternalNotes = CleanString(objTicket.InternalNotes);

                objTicket.Entity = new CrmEntity
                {
                    Id = objTicketId,
                    EntityTypeId = (Int32)PredefinedEntityType.Ticket,
                    DisplayName = strTitle,
                    OwnerUserId = objTicket.AssignedToUserId ?? objUserId,
                    CreatedUtc = dteNow,
                    CreatedByUserId = objUserId
                };

                objTicket.StatusHistory.Add(new TicketStatusHistory
                {
                    Id = Guid.NewGuid(),
                    TicketId = objTicketId,
                    PreviousStatus = null,
                    NewStatus = objTicket.Status,
                    ChangedByUserId = objUserId,
                    ChangedUtc = dteNow
                });

                await _ticketRepository.AddTicketAsync(objTicket, objToken);
                await _ticketRepository.SaveChangesAsync(objToken);

                return objTicket;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                        objError,
                        LogArea.Tickets,
                        objUserId,
                        objTicket?.Id,
                        ItemType.None,
                        "Failed to add ticket.",
                        objToken);

                return null;
            }
        }

        ///<inheritdoc/>
        public async Task<Ticket?> UpdateTicketAsync(Ticket objTicket, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objToken.IsCancellationRequested)
                {
                    return null;
                }

                if (objTicket == null)
                {
                    await _logService.LogErrorAsync(
                        new ArgumentNullException(nameof(objTicket)),
                        LogArea.Tickets,
                        objUserId,
                        objTicket?.Id,
                        ItemType.None,
                        "Ticket could not be updated because the model was null.",
                        objToken);

                    return null;
                }

                if (objTicket.Id == Guid.Empty)
                {
                    await _logService.LogErrorAsync(
                        new ArgumentException("Ticket id is required.", nameof(objTicket)),
                        LogArea.Tickets,
                        objUserId,
                        objTicket?.Id,
                        ItemType.None,
                        "Ticket could not be updated because the id was empty.",
                        objToken);

                    return null;
                }

                if (String.IsNullOrWhiteSpace(objTicket.Title))
                {
                    await _logService.LogErrorAsync(
                        new ArgumentException("Ticket title is required.", nameof(objTicket)),
                        LogArea.Tickets,
                        objUserId,
                        objTicket?.Id,
                        ItemType.None,
                        "Ticket could not be updated because the title was empty.",
                        objToken);

                    return null;
                }

                Ticket? objExistingTicket = await _ticketRepository.GetTicketByIdAsync(objTicket.Id, true, objToken);

                if (objExistingTicket == null || objExistingTicket.Entity.DeletedUtc.HasValue)
                {
                    return null;
                }

                DateTime dteNow = DateTime.UtcNow;
                String strTitle = objTicket.Title.Trim();

                TicketStatus enmPreviousStatus = objExistingTicket.Status;

                objExistingTicket.Title = strTitle;
                objExistingTicket.Description = CleanString(objTicket.Description);

                objExistingTicket.CompanyId = objTicket.CompanyId;
                objExistingTicket.ContactId = objTicket.ContactId;
                objExistingTicket.JobId = objTicket.JobId;
                objExistingTicket.ProjectId = objTicket.ProjectId;

                objExistingTicket.AssignedToUserId = objTicket.AssignedToUserId;

                objExistingTicket.Type = objTicket.Type;
                objExistingTicket.Status = objTicket.Status;
                objExistingTicket.Priority = objTicket.Priority;

                objExistingTicket.IsChargeable = objTicket.IsChargeable;
                objExistingTicket.IsInvoiced = objTicket.IsInvoiced;

                objExistingTicket.EstimatedValue = objTicket.EstimatedValue;
                objExistingTicket.QuotedValue = objTicket.QuotedValue;
                objExistingTicket.InvoiceValue = objTicket.InvoiceValue;

                objExistingTicket.EstimatedMinutes = objTicket.EstimatedMinutes;
                objExistingTicket.DueUtc = objTicket.DueUtc;

                objExistingTicket.InternalNotes = CleanString(objTicket.InternalNotes);

                if (objExistingTicket.Status == TicketStatus.Complete && objExistingTicket.CompletedUtc == null)
                {
                    objExistingTicket.CompletedUtc = dteNow;
                    objExistingTicket.CompletedByUserId = objUserId;
                }

                if (objExistingTicket.Status != TicketStatus.Complete)
                {
                    objExistingTicket.CompletedUtc = null;
                    objExistingTicket.CompletedByUserId = null;
                }

                objExistingTicket.Entity.DisplayName = strTitle;
                objExistingTicket.Entity.OwnerUserId = objExistingTicket.AssignedToUserId ?? objExistingTicket.Entity.OwnerUserId;
                objExistingTicket.Entity.UpdatedUtc = dteNow;
                objExistingTicket.Entity.UpdatedByUserId = objUserId;

                if (enmPreviousStatus != objExistingTicket.Status)
                {
                    await _ticketRepository.AddTicketStatusHistoryAsync(new TicketStatusHistory
                    {
                        Id = Guid.NewGuid(),
                        TicketId = objExistingTicket.Id,
                        PreviousStatus = enmPreviousStatus,
                        NewStatus = objExistingTicket.Status,
                        ChangedByUserId = objUserId,
                        ChangedUtc = dteNow
                    }, objToken);
                }

                await _ticketRepository.SaveChangesAsync(objToken);

                return objExistingTicket;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicket?.Id,
                    ItemType.None,
                    "Failed to update ticket.",
                    objToken);

                return null;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> SetStatusAsync(Guid objTicketId, TicketStatus enmStatus, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                if (objTicket.Status == enmStatus)
                {
                    return true;
                }

                DateTime dteNow = DateTime.UtcNow;
                TicketStatus enmPreviousStatus = objTicket.Status;

                objTicket.Status = enmStatus;

                if (enmStatus == TicketStatus.Complete)
                {
                    objTicket.CompletedUtc = dteNow;
                    objTicket.CompletedByUserId = objUserId;
                }
                else
                {
                    objTicket.CompletedUtc = null;
                    objTicket.CompletedByUserId = null;
                }

                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.AddTicketStatusHistoryAsync(new TicketStatusHistory
                {
                    Id = Guid.NewGuid(),
                    TicketId = objTicket.Id,
                    PreviousStatus = enmPreviousStatus,
                    NewStatus = enmStatus,
                    ChangedByUserId = objUserId,
                    ChangedUtc = dteNow
                }, objToken);

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to set ticket status.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> MarkInvoicedAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                objTicket.IsInvoiced = true;
                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to mark ticket as invoiced.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> AddCommentAsync(Guid objTicketId, String strComment, Boolean blnIsInternal = true, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(strComment))
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                await _ticketRepository.AddTicketCommentAsync(new TicketComment
                {
                    Id = Guid.NewGuid(),
                    TicketId = objTicketId,
                    CreatedByUserId = objUserId,
                    CreatedUtc = dteNow,
                    IsInternal = blnIsInternal,
                    Text = strComment.Trim()
                }, objToken);

                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to add ticket comment.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> AddTimeEntryAsync(Guid objTicketId, Int32 intMinutes, String? strNotes = null, Boolean blnIsChargeable = true, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                if (intMinutes <= 0)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                await _ticketRepository.AddTicketTimeEntryAsync(new TicketTimeEntry
                {
                    Id = Guid.NewGuid(),
                    TicketId = objTicketId,
                    UserId = objUserId,
                    CreatedUtc = dteNow,
                    WorkDateUtc = dteNow,
                    Minutes = intMinutes,
                    IsChargeable = blnIsChargeable,
                    Notes = CleanString(strNotes)
                }, objToken);

                objTicket.ActualMinutes += intMinutes;
                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to add ticket time entry.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> ArchiveTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                objTicket.Entity.ArchivedUtc = dteNow;
                objTicket.Entity.ArchivedByUserId = objUserId;
                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to archive ticket.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> RestoreTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                objTicket.Entity.ArchivedUtc = null;
                objTicket.Entity.ArchivedByUserId = null;
                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to restore ticket.",
                    objToken);

                return false;
            }
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteTicketAsync(Guid objTicketId, Guid? objUserId = null, CancellationToken objToken = default)
        {
            try
            {
                if (objTicketId == Guid.Empty || objToken.IsCancellationRequested)
                {
                    return false;
                }

                Ticket? objTicket = await _ticketRepository.GetTicketByIdAsync(objTicketId, true, objToken);

                if (objTicket == null || objTicket.Entity.DeletedUtc.HasValue)
                {
                    return false;
                }

                DateTime dteNow = DateTime.UtcNow;

                objTicket.Entity.DeletedUtc = dteNow;
                objTicket.Entity.DeletedByUserId = objUserId;
                objTicket.Entity.UpdatedUtc = dteNow;
                objTicket.Entity.UpdatedByUserId = objUserId;

                await _ticketRepository.SaveChangesAsync(objToken);

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception objError)
            {
                await _logService.LogErrorAsync(
                    objError,
                    LogArea.Tickets,
                    objUserId,
                    objTicketId,
                    ItemType.None,
                    "Failed to delete ticket.",
                    objToken);

                return false;
            }
        }

        /// <summary>
        /// Cleans a string by trimming whitespace and returning null if the string is null or whitespace.
        /// todo: look for all other emthods liek this and replace with a common helper method
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private static String? CleanString(String? strValue)
        {
            if (String.IsNullOrWhiteSpace(strValue))
            {
                return null;
            }

            return strValue.Trim();
        }
    }
}