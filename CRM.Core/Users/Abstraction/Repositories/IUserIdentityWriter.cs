using CRM.Contracts.Results;
using CRM.Core.Users.Domain;

namespace CRM.Core.Users.Abstraction.Repositories
{
    public interface IUserIdentityWriter
    {
        Task<BasicResult> UpdateUserAsync(
            UpdateUserRequest objRequest,
            CancellationToken objToken = default);

        //Task<BasicResult> SetAccessLevelAsync(
        //    Guid objUserId,
        //    UserAccessLevel enmAccessLevel,
        //    CancellationToken objToken = default);

        Task<BasicResult> AddOrUpdateClaimAsync(
            Guid objUserId,
            String strKey,
            String strValue,
            CancellationToken objToken = default);

        Task<BasicResult<String>> GenerateEmailConfirmationTokenAsync(
            Guid objUserId,
            CancellationToken objToken = default);
    }
}
