using CRM.Core.Logging.Abstraction;
using CRM.Core.Users.Abstraction.Repositories;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;

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

        public Task<User?> GetUserAsync(
            Guid objUserId,
            CancellationToken objToken = default)
        {
            if (objUserId == Guid.Empty)
            {
                return Task.FromResult<User?>(null);
            }

            return _userRepository.GetUserAsync(objUserId, objToken);
        }
    }
}