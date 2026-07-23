using CRM.Contracts.Results;
using CRM.Core.Users.Domain;

namespace CRM.Core.Users.Abstraction.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Gets a dictionary of user display names by their user IDs.
        /// </summary>
        /// <param name="colUserIds"></param>
        /// <param name="objToken"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, String>> GetDisplayNamesByUserIdsAsync(
            IReadOnlyCollection<Guid> colUserIds,
            CancellationToken objToken = default);

        Task<User?> GetUserAsync(
            Guid objUserId,
            CancellationToken objToken = default);

        Task<BasicResult> UpdateUserAsync(
            UpdateUserRequest objRequest,
            CancellationToken objToken = default);
    }
}
