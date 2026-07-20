using CRM.Core.Notes.Domain;

namespace CRM.Core.Notes.Abstractions
{
    public interface INoteRepository
    {
        /// <summary>
        /// Determines whether an entity with the specified ID exists in the repository.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Boolean> EntityExistsAsync(Guid objEntityId, CancellationToken objToken = default);

        /// <summary>
        /// Retrieves a list of notes associated with the specified entity ID, with options to limit the number of results and include deleted notes.
        /// </summary>
        /// <param name="objEntityId"></param>
        /// <param name="intTake"></param>
        /// <param name="blnIncludeDeleted"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Retrieves a list of recent notes, optionally filtered by a search string and limited to a specified number of results.
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
        /// Adds a new note to the repository.
        /// </summary>
        /// <param name="objNote"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task AddNoteAsync(Note objNote, CancellationToken objToken = default);

        /// <summary>
        /// Retrieves a note by its unique identifier, with an option to enable or disable tracking of the entity.
        /// </summary>
        /// <param name="objNoteId"></param>
        /// <param name="blnTracking"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Note?> GetNoteByIdAsync(
            Guid objNoteId,
            Boolean blnTracking = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Deletes a note from the repository.
        /// </summary>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken objToken = default);
    }
}