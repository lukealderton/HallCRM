using CRM.Core.Notes.Abstractions;
using CRM.Core.Notes.Domain;

namespace CRM.Core.Notes.Services
{
    public sealed class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository objNoteRepository)
        {
            _noteRepository = objNoteRepository;
        }

        ///<inheritdoc/>
        public Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            CancellationToken objToken = default)
        {
            if (objEntityId == Guid.Empty)
            {
                throw new ArgumentException("Entity id is required.", nameof(objEntityId));
            }

            return _noteRepository.GetNotesForEntityAsync(objEntityId, intTake, false, objToken);
        }

        ///<inheritdoc/>
        public Task<List<Note>> GetRecentNotesAsync(
            String? strSearch = null,
            Int32 intTake = 100,
            CancellationToken objToken = default)
        {
            return _noteRepository.GetRecentNotesAsync(strSearch, intTake, objToken);
        }

        ///<inheritdoc/>
        public async Task<Note> AddNoteAsync(
            Guid objEntityId,
            String strBody,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            if (objEntityId == Guid.Empty)
            {
                throw new ArgumentException("Entity id is required.", nameof(objEntityId));
            }

            if (String.IsNullOrWhiteSpace(strBody))
            {
                throw new ArgumentException("Note text is required.", nameof(strBody));
            }

            Boolean blnEntityExists = await _noteRepository.EntityExistsAsync(objEntityId, objToken);

            if (!blnEntityExists)
            {
                throw new InvalidOperationException("The entity could not be found.");
            }

            DateTime dtmNow = DateTime.UtcNow;

            Note objNote = new()
            {
                Id = Guid.NewGuid(),
                EntityId = objEntityId,
                Body = strBody.Trim(),
                CreatedUtc = dtmNow,
                CreatedByUserId = objUserId
            };

            await _noteRepository.AddNoteAsync(objNote, objToken);
            await _noteRepository.SaveChangesAsync(objToken);

            return objNote;
        }

        ///<inheritdoc/>
        public async Task<Boolean> DeleteNoteAsync(
            Guid objNoteId,
            Guid? objUserId = null,
            CancellationToken objToken = default)
        {
            Note? objNote = await _noteRepository.GetNoteByIdAsync(objNoteId, true, objToken);

            if (objNote == null || objNote.DeletedUtc.HasValue)
            {
                return false;
            }

            DateTime dtmNow = DateTime.UtcNow;

            objNote.DeletedUtc = dtmNow;
            objNote.DeletedByUserId = objUserId;
            objNote.UpdatedUtc = dtmNow;
            objNote.UpdatedByUserId = objUserId;

            await _noteRepository.SaveChangesAsync(objToken);

            return true;
        }
    }
}