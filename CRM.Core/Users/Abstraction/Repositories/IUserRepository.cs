using CRM.Contracts.Results;
using CRM.Core.Users.Domain;

namespace CRM.Core.Users.Abstraction.Repositories
{
    public interface IUserRepository
    {
        Task<Dictionary<Guid, String>> GetDisplayNamesByUserIdsAsync(
            IReadOnlyCollection<Guid> colUserIds,
            CancellationToken objToken = default);

        Task<User?> GetUserAsync(
            Guid objUserId,
            CancellationToken objToken = default);

        Task<List<User>> GetUsersAsync(
            IEnumerable<Guid> colUserIds,
            CancellationToken objToken = default);

        Task<List<User>> GetUsersAsync(
            Boolean blnIncludeDisabled = false,
            CancellationToken objToken = default);

        Task<BasicResult> UpdateUserAsync(
            UpdateUserRequest objRequest,
            CancellationToken objToken = default);
    }
}