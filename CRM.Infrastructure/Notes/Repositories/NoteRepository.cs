using CRM.Core.Notes.Abstractions;
using CRM.Core.Notes.Domain;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Notes.Repositories
{
    public sealed class NoteRepository : INoteRepository
    {
        private readonly CRMDbContext _dbContext;

        public NoteRepository(CRMDbContext objDbContext)
        {
            _dbContext = objDbContext;
        }

        public Task<Boolean> EntityExistsAsync(Guid objEntityId, CancellationToken objToken = default)
        {
            return _dbContext.Entities
                .AnyAsync(x => x.Id == objEntityId && !x.DeletedUtc.HasValue, objToken);
        }

        public async Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            IQueryable<Note> qryNotes = _dbContext.Notes
                .AsNoTracking()
                .Where(x => x.EntityId == objEntityId);

            if (!blnIncludeDeleted)
            {
                qryNotes = qryNotes.Where(x => !x.DeletedUtc.HasValue);
            }

            List<Note> colNotes = await qryNotes
                .OrderByDescending(x => x.CreatedUtc)
                .Take(intTake)
                .ToListAsync(objToken);

            await PopulateAuthorDisplayNamesAsync(colNotes, objToken);

            return colNotes;
        }

        public async Task<List<Note>> GetRecentNotesAsync(
            String? strSearch = null,
            Int32 intTake = 100,
            CancellationToken objToken = default)
        {
            IQueryable<Note> qryNotes = _dbContext.Notes
                .AsNoTracking()
                .Include(x => x.Entity)
                    .ThenInclude(x => x!.EntityType)
                .Where(x => !x.DeletedUtc.HasValue);

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strCleanSearch = strSearch.Trim();

                qryNotes = qryNotes.Where(x =>
                    x.Body.Contains(strCleanSearch) ||
                    x.Entity!.DisplayName.Contains(strCleanSearch));
            }

            List<Note> colNotes = await qryNotes
                .OrderByDescending(x => x.CreatedUtc)
                .Take(intTake)
                .ToListAsync(objToken);

            await PopulateAuthorDisplayNamesAsync(colNotes, objToken);

            return colNotes;
        }

        public Task AddNoteAsync(Note objNote, CancellationToken objToken = default)
        {
            return _dbContext.Notes.AddAsync(objNote, objToken).AsTask();
        }

        public Task<Note?> GetNoteByIdAsync(Guid objNoteId, Boolean blnTracking = false, CancellationToken objToken = default)
        {
            IQueryable<Note> qryNotes = _dbContext.Notes;

            if (!blnTracking)
            {
                qryNotes = qryNotes.AsNoTracking();
            }

            return qryNotes.FirstOrDefaultAsync(x => x.Id == objNoteId, objToken);
        }

        public Task SaveChangesAsync(CancellationToken objToken = default)
        {
            return _dbContext.SaveChangesAsync(objToken);
        }

        private async Task PopulateAuthorDisplayNamesAsync(
            List<Note> colNotes,
            CancellationToken objToken = default)
        {
            List<Guid> colUserIds = colNotes
                .Where(x => x.CreatedByUserId.HasValue && x.CreatedByUserId.Value != Guid.Empty)
                .Select(x => x.CreatedByUserId!.Value)
                .Distinct()
                .ToList();

            if (colUserIds.Count == 0)
            {
                return;
            }

            var colUsers = await _dbContext.Set<ApplicationUser>()
                .AsNoTracking()
                .Where(x => colUserIds.Contains(x.DomainUserId))
                .Select(x => new
                {
                    x.DomainUserId,
                    x.Email,
                    x.UserName
                })
                .ToListAsync(objToken);

            Dictionary<Guid, String> colDisplayNames = colUsers
                .ToDictionary(
                    x => x.DomainUserId,
                    x => GetUserDisplayName(x.Email, x.UserName));

            foreach (Note objNote in colNotes)
            {
                if (!objNote.CreatedByUserId.HasValue)
                {
                    continue;
                }

                if (colDisplayNames.TryGetValue(objNote.CreatedByUserId.Value, out String? strDisplayName))
                {
                    objNote.AuthorDisplayName = strDisplayName;
                }
            }
        }

        private static String GetUserDisplayName(String? strEmail, String? strUserName)
        {
            if (!String.IsNullOrWhiteSpace(strEmail))
            {
                return strEmail.Trim();
            }

            if (!String.IsNullOrWhiteSpace(strUserName))
            {
                return strUserName.Trim();
            }

            return "Unknown user";
        }
    }
}