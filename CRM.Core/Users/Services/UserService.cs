using CRM.Core.Logging.Abstraction;
using CRM.Core.Users.Abstraction.Repositories;
using CRM.Core.Users.Abstraction.Services;

namespace CRM.Core.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository    _userRepository;
        private readonly ILogService        _logService;

        public UserService(
            ILogService     objLogService,
            IUserRepository objUserRepository)
        {
            _logService     = objLogService;
            _userRepository = objUserRepository;
        }

        ///<inheritdoc/>
        public Task<Dictionary<Guid, String>> GetDisplayNamesByUserIdsAsync(
            IReadOnlyCollection<Guid> colUserIds,
            CancellationToken objToken = default)
        {
            if (colUserIds == null || colUserIds.Count == 0)
            {
                return Task.FromResult(new Dictionary<Guid, String>());
            }

            return _userRepository.GetDisplayNamesByUserIdsAsync(colUserIds, objToken);
        }
    }
}