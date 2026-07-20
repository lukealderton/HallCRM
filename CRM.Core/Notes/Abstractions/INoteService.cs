using CRM.Core.Notes.Domain;

namespace CRM.Core.Notes.Abstractions
{
    public interface INoteService
    {
        /// <summary>
        /// Gets the notes for a specific entity.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="intTake"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            CancellationToken objToken = default);

        /// <summary>
        /// Gets the most recent notes across all entities, optionally filtered by a search string.
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="intTake"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Note>> GetRecentNotesAsync(
            String? strSearch = null,
            Int32 intTake = 100,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new note to a specific entity.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="strBody"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Note> AddNoteAsync(
            Guid objEntityId,
            String strBody,
            Guid? objUserId = null,
            CancellationToken objToken = default);

        /// <summary>
        /// Deletes a note by its unique identifier.
        /// </summary>
        /// <param name="objNoteId"></param>
        /// <param name="objUserId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> DeleteNoteAsync(
            Guid objNoteId,
            Guid? objUserId = null,
            CancellationToken objToken = default);
    }
}