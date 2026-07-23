using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;
using CRM.Core.Users.Services;
using CRM.Web.Users.Abstraction;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CRM.Web.Users.Services
{
    public sealed class CurrentUserState : ICurrentUserState
    {
        private readonly IUserService _userService;
        private readonly AuthenticationStateProvider _objAuthenticationStateProvider;
        private readonly SemaphoreSlim _objGate = new(1, 1);

        private User? _objCurrentUser;
        private Guid? _objCurrentUserId;

        public CurrentUserState(
            UserService objUserService,
            AuthenticationStateProvider objAuthenticationStateProvider)
        {
            _userService = objUserService;
            _objAuthenticationStateProvider = objAuthenticationStateProvider;
        }

        public Guid? CurrentUserId => _objCurrentUserId;
        public User? CurrentUser => _objCurrentUser;

        public async Task<Guid?> GetCurrentUserIdAsync(CancellationToken objToken = default)
        {
            if (_objCurrentUserId != null)
            {
                return _objCurrentUserId;
            }

            await _objGate.WaitAsync(objToken);
            try
            {
                if (_objCurrentUserId != null)
                {
                    return _objCurrentUserId;
                }

                AuthenticationState objAuthenticationState = await _objAuthenticationStateProvider.GetAuthenticationStateAsync();
                ClaimsPrincipal objPrincipal = objAuthenticationState.User;

                if (objPrincipal?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                String? strUserId = objPrincipal.FindFirst("Id")?.Value
                    ?? objPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(strUserId, out Guid objUserId) || objUserId == Guid.Empty)
                {
                    return null;
                }

                _objCurrentUserId = objUserId;
                return _objCurrentUserId;
            }
            finally
            {
                _objGate.Release();
            }
        }

        public async Task<User?> GetAsync(CancellationToken objToken = default)
        {
            if (_objCurrentUser != null)
            {
                return _objCurrentUser;
            }

            Guid? objUserId = await GetCurrentUserIdAsync(objToken);
            if (objUserId == null || objUserId == Guid.Empty)
            {
                return null;
            }

            await _objGate.WaitAsync(objToken);
            try
            {
                if (_objCurrentUser != null)
                {
                    return _objCurrentUser;
                }

                _objCurrentUser = await _userService.GetUserAsync((Guid)objUserId, objToken);
                return _objCurrentUser;
            }
            finally
            {
                _objGate.Release();
            }
        }

        public async Task<User?> RefreshAsync(CancellationToken objToken = default)
        {
            Guid? objUserId = await GetCurrentUserIdAsync(objToken);

            if (objUserId == null || objUserId == Guid.Empty)
            {
                _objCurrentUser = null;
                return null;
            }

            await _objGate.WaitAsync(objToken);
            try
            {
                _objCurrentUser = await _userService.GetUserAsync((Guid)objUserId, objToken);
                return _objCurrentUser;
            }
            finally
            {
                _objGate.Release();
            }
        }

        public void Set(User? objUser)
        {
            _objCurrentUser = objUser;
            _objCurrentUserId = objUser?.Id;
        }

        public void Clear()
        {
            _objCurrentUser = null;
            _objCurrentUserId = null;
        }
    }
}