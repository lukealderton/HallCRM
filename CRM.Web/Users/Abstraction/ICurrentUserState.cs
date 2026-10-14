using CRM.Core.Users.Domain;

namespace CRM.Web.Users.Abstraction
{
    public interface ICurrentUserState
    {
        /// <summary>
        /// Gets the CRM domain user ID used by entity audit fields, not the login account ID.
        /// </summary>
        Task<Guid?> GetCurrentUserIdAsync(CancellationToken objToken = default);
        Task<User?> GetAsync(CancellationToken token = default);
        Task<User?> RefreshAsync(CancellationToken token = default);

        /// <summary>
        /// Set immediately after login
        /// </summary>
        /// <param name="user"></param>
        void Set(User? user);

        /// <summary>
        /// Call on sign out
        /// </summary>
        void Clear();

        /// <summary>
        /// The current CRM domain user ID
        /// </summary>
        Guid? CurrentUserId { get; }

        /// <summary>
        /// The current User - may not be set until GetAsync or RefreshAsync is called, and may be null if the user is not authenticated
        /// </summary>
        User? CurrentUser { get; }
    }
}
