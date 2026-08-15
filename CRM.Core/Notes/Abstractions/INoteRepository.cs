using CRM.Core.Notes.Domain;

namespace CRM.Core.Notes.Abstractions
{
    public interface INoteRepository
    {
        /// <summary>
        /// Determines whether an entity with the specified ID exists
        /// and has not been deleted.
        /// </summary>
        Task<Boolean> EntityExistsAsync(
            Guid objEntityId,
            CancellationToken objToken = default);

        /// <summary>
        /// Retrieves notes associated with the specified entity.
        /// </summary>
        Task<List<Note>> GetNotesForEntityAsync(
            Guid objEntityId,
            Int32 intTake = 50,
            Boolean blnIncludeDeleted = false,
            CancellationToken objToken = default);

        /// <summary>
        /// Retrieves recent notes, optionally filtered by search text.
        /// </summary>
        Task<List<Note>> GetRecentNotesAsync(
            String? strSearch = null,
            Int32 intTake = 100,
            CancellationToken objToken = default);

        /// <summary>
        /// Adds a new note and persists it.
        /// </summary>
        Task AddNoteAsync(
            Note objNote,
            CancellationToken objToken = default);

        /// <summary>
        /// Retrieves a note by its unique identifier.
        /// </summary>
        Task<Note?> GetNoteByIdAsync(
            Guid objNoteId,
            CancellationToken objToken = default);

        /// <summary>
        /// Updates an existing note and persists it.
        /// </summary>
        Task UpdateNoteAsync(
            Note objNote,
            CancellationToken objToken = default);
    }
}