using CRM.Core.Tickets.Abstractions;
using CRM.Core.Tickets.Domain;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Tickets.Repositories
{
    public sealed class TicketRepository : ITicketRepository
    {
        private readonly CRMDbContext _dbContext;

        public TicketRepository(CRMDbContext objDbContext)
        {
            _dbContext = objDbContext;
        }

        public async Task<Ticket?> GetTicketByIdAsync(Guid objTicketId, Boolean blnTrackChanges = false, CancellationToken objToken = default)
        {
            IQueryable<Ticket> objQuery = _dbContext.Set<Ticket>()
                .Include(objTicket => objTicket.Entity)
                .Include(objTicket => objTicket.Company)
                .Include(objTicket => objTicket.Comments.OrderByDescending(objComment => objComment.CreatedUtc))
                .Include(objTicket => objTicket.TimeEntries.OrderByDescending(objTimeEntry => objTimeEntry.CreatedUtc))
                .Include(objTicket => objTicket.StatusHistory.OrderByDescending(objHistory => objHistory.ChangedUtc));

            if (!blnTrackChanges)
            {
                objQuery = objQuery.AsNoTracking();
            }

            return await objQuery.FirstOrDefaultAsync(objTicket => objTicket.Id == objTicketId, objToken);
        }

        public async Task<TicketSearchResult> GetTicketsAsync(TicketSearchRequest objRequest, CancellationToken objToken = default)
        {
            IQueryable<Ticket> objQuery = _dbContext.Set<Ticket>()
                .Include(objTicket => objTicket.Entity)
                .Include(objTicket => objTicket.Company);

            if (!objRequest.IncludeDeleted)
            {
                objQuery = objQuery.Where(objTicket => objTicket.Entity.DeletedUtc == null);
            }

            if (!objRequest.IncludeArchived)
            {
                objQuery = objQuery.Where(objTicket => objTicket.Entity.ArchivedUtc == null);
            }

            if (!String.IsNullOrWhiteSpace(objRequest.Search))
            {
                String strSearch = objRequest.Search.Trim();

                objQuery = objQuery.Where(objTicket =>
                    objTicket.Title.Contains(strSearch) ||
                    (objTicket.Description != null && objTicket.Description.Contains(strSearch)) ||
                    (objTicket.Company != null && objTicket.Company.Name.Contains(strSearch)));
            }

            if (objRequest.CompanyId.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.CompanyId == objRequest.CompanyId.Value);
            }

            if (objRequest.ContactId.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.ContactId == objRequest.ContactId.Value);
            }

            if (objRequest.AssignedToUserId.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.AssignedToUserId == objRequest.AssignedToUserId.Value);
            }

            if (objRequest.Status.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.Status == objRequest.Status.Value);
            }

            if (objRequest.Type.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.Type == objRequest.Type.Value);
            }

            if (objRequest.Priority.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.Priority == objRequest.Priority.Value);
            }

            if (objRequest.IsChargeable.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.IsChargeable == objRequest.IsChargeable.Value);
            }

            if (objRequest.IsInvoiced.HasValue)
            {
                objQuery = objQuery.Where(objTicket => objTicket.IsInvoiced == objRequest.IsInvoiced.Value);
            }

            if (objRequest.HideInvoiced)
            {
                objQuery = objQuery.Where(objTicket => !objTicket.IsInvoiced);
            }

            Int32 intTotalCount = await objQuery.CountAsync(objToken);

            List<Ticket> colTickets = await objQuery
                .AsNoTracking()
                .OrderByDescending(objTicket => objTicket.Entity.CreatedUtc)
                .Skip(objRequest.Skip)
                .Take(objRequest.PageSize)
                .ToListAsync(objToken);

            return new TicketSearchResult
            {
                Tickets = colTickets,
                TotalCount = intTotalCount
            };
        }

        public async Task AddTicketAsync(Ticket objTicket, CancellationToken objToken = default)
        {
            await _dbContext.Set<Ticket>().AddAsync(objTicket, objToken);
        }

        public async Task AddTicketCommentAsync(TicketComment objComment, CancellationToken objToken = default)
        {
            await _dbContext.Set<TicketComment>().AddAsync(objComment, objToken);
        }

        public async Task AddTicketTimeEntryAsync(TicketTimeEntry objTimeEntry, CancellationToken objToken = default)
        {
            await _dbContext.Set<TicketTimeEntry>().AddAsync(objTimeEntry, objToken);
        }

        public async Task AddTicketStatusHistoryAsync(TicketStatusHistory objStatusHistory, CancellationToken objToken = default)
        {
            await _dbContext.Set<TicketStatusHistory>().AddAsync(objStatusHistory, objToken);
        }

        public Task SaveChangesAsync(CancellationToken objToken = default)
        {
            return _dbContext.SaveChangesAsync(objToken);
        }
    }
}