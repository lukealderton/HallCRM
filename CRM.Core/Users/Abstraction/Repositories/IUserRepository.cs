namespace CRM.Core.Users.Abstraction.Repositories
{
    public interface IUserRepository
    {
        Task<Dictionary<Guid, String>> GetDisplayNamesByUserIdsAsync(
            IReadOnlyCollection<Guid> colUserIds,
            CancellationToken objToken = default);
    }
}