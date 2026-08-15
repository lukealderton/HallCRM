using CRM.Core.Notes.Abstractions;
using CRM.Core.Notes.Domain;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Notes.Repositories
{
    public sealed class NoteRepository : INoteRepository
    {
        private readonly IDbContextFactory<CRMDbContext> _objDbContextFactory;

        public NoteRepository(
            IDbContextFactory<CRMDbContext> objDbContextFactory)
        {
            _objDbContextFactory = objDbContextFactory;
        }

        ///<inheritdoc/>
        public async Task<Boolean> EntityExistsAsync(
            Guid objEntityId,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Entities
                .AsNoTracking()
                .AnyAsync(
                    objEntity =>
                        objEntity.Id == objEntityId &&
                        !objEntity.DeletedUtc.HasValue,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Note> qryNotes =
                objDbContext.Notes
                    .AsNoTracking()
                    .Where(objNote =>
                        objNote.EntityId == objEntityId);

            if (!blnIncludeDeleted)
            {
                qryNotes = qryNotes.Where(objNote =>
                    !objNote.DeletedUtc.HasValue);
            }

            List<Note> colNotes =
                await qryNotes
                    .OrderByDescending(objNote =>
                        objNote.CreatedUtc)
                    .Take(intTake)
                    .ToListAsync(objToken);

            await PopulateAuthorDisplayNamesAsync(
                colNotes,
                objToken);

            return colNotes;
        }

        ///<inheritdoc/>
        public async Task<List<Note>> GetRecentNotesAsync(
            String? strSearch = null,
            Int32 intTake = 100,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            IQueryable<Note> qryNotes =
                objDbContext.Notes
                    .AsNoTracking()
                    .Include(objNote => objNote.Entity)
                        .ThenInclude(objEntity =>
                            objEntity!.EntityType)
                    .Where(objNote =>
                        !objNote.DeletedUtc.HasValue);

            if (!String.IsNullOrWhiteSpace(strSearch))
            {
                String strCleanSearch =
                    strSearch.Trim();

                qryNotes = qryNotes.Where(objNote =>
                    objNote.Body.Contains(strCleanSearch) ||
                    (
                        objNote.Entity != null &&
                        objNote.Entity.DisplayName.Contains(strCleanSearch)
                    ));
            }

            List<Note> colNotes =
                await qryNotes
                    .OrderByDescending(objNote =>
                        objNote.CreatedUtc)
                    .Take(intTake)
                    .ToListAsync(objToken);

            await PopulateAuthorDisplayNamesAsync(
                colNotes,
                objToken);

            return colNotes;
        }

        ///<inheritdoc/>
        public async Task AddNoteAsync(
            Note objNote,
            CancellationToken objToken = default)
        {
            if (objNote == null)
            {
                throw new ArgumentNullException(nameof(objNote));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            await objDbContext.Notes.AddAsync(
                objNote,
                objToken);

            await objDbContext.SaveChangesAsync(objToken);
        }

        ///<inheritdoc/>
        public async Task<Note?> GetNoteByIdAsync(
            Guid objNoteId,
            CancellationToken objToken = default)
        {
            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            return await objDbContext.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    objNote => objNote.Id == objNoteId,
                    objToken);
        }

        ///<inheritdoc/>
        public async Task UpdateNoteAsync(
            Note objNote,
            CancellationToken objToken = default)
        {
            if (objNote == null)
            {
                throw new ArgumentNullException(nameof(objNote));
            }

            if (objNote.Id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Note id is required.",
                    nameof(objNote));
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            objDbContext.Entry(objNote).State =
                EntityState.Modified;

            await objDbContext.SaveChangesAsync(objToken);
        }

        private async Task PopulateAuthorDisplayNamesAsync(
            List<Note> colNotes,
            CancellationToken objToken = default)
        {
            List<Guid> colUserIds =
                colNotes
                    .Where(objNote =>
                        objNote.CreatedByUserId.HasValue &&
                        objNote.CreatedByUserId.Value != Guid.Empty)
                    .Select(objNote =>
                        objNote.CreatedByUserId!.Value)
                    .Distinct()
                    .ToList();

            if (colUserIds.Count == 0)
            {
                return;
            }

            await using CRMDbContext objDbContext =
                await _objDbContextFactory.CreateDbContextAsync(objToken);

            var colUsers =
                await objDbContext
                    .Set<ApplicationUser>()
                    .AsNoTracking()
                    .Where(objUser =>
                        colUserIds.Contains(objUser.DomainUserId))
                    .Select(objUser => new
                    {
                        objUser.DomainUserId,
                        objUser.Email,
                        objUser.UserName
                    })
                    .ToListAsync(objToken);

            Dictionary<Guid, String> colDisplayNames =
                colUsers.ToDictionary(
                    objUser => objUser.DomainUserId,
                    objUser => GetUserDisplayName(
                        objUser.Email,
                        objUser.UserName));

            foreach (Note objNote in colNotes)
            {
                if (!objNote.CreatedByUserId.HasValue)
                {
                    continue;
                }

                if (colDisplayNames.TryGetValue(
                    objNote.CreatedByUserId.Value,
                    out String? strDisplayName))
                {
                    objNote.AuthorDisplayName =
                        strDisplayName;
                }
            }
        }

        private static String GetUserDisplayName(
            String? strEmail,
            String? strUserName)
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