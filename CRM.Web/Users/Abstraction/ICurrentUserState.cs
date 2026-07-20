using CRM.Infrastructure.Identity;

namespace CRM.Web.Users.Abstraction
{
    public interface ICurrentUserState
    {
        Task<Guid?> GetCurrentUserIdAsync(CancellationToken objToken = default);
        Task<ApplicationUser?> GetAsync(CancellationToken token = default);
        Task<ApplicationUser?> RefreshAsync(CancellationToken token = default);

        /// <summary>
        /// Set immediately after login
        /// </summary>
        /// <param name="user"></param>
        void Set(ApplicationUser? user);

        /// <summary>
        /// Call on sign out
        /// </summary>
        void Clear();

        /// <summary>
        /// The current user id
        /// </summary>
        Guid? CurrentUserId { get; }

        /// <summary>
        /// The current User - may not be set until GetAsync or RefreshAsync is called, and may be null if the user is not authenticated
        /// </summary>
        ApplicationUser? CurrentUser { get; }
    }
}