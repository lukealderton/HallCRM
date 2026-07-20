using CRM.Infrastructure.Identity;
using CRM.Web.Users.Abstraction;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CRM.Web.Users.Services
{
    public sealed class CurrentUserState : ICurrentUserState
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthenticationStateProvider _objAuthenticationStateProvider;
        private readonly SemaphoreSlim _objGate = new(1, 1);

        private ApplicationUser? _objCurrentUser;
        private Guid? _objCurrentUserId;

        public CurrentUserState(
            UserManager<ApplicationUser> objUserManager,
            AuthenticationStateProvider objAuthenticationStateProvider)
        {
            _userManager = objUserManager;
            _objAuthenticationStateProvider = objAuthenticationStateProvider;
        }

        public Guid? CurrentUserId => _objCurrentUserId;
        public ApplicationUser? CurrentUser => _objCurrentUser;

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

        public async Task<ApplicationUser?> GetAsync(CancellationToken objToken = default)
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

                _objCurrentUser = _userManager.Users.FirstOrDefault(x => x.DomainUserId == objUserId);
                return _objCurrentUser;
            }
            finally
            {
                _objGate.Release();
            }
        }

        public async Task<ApplicationUser?> RefreshAsync(CancellationToken objToken = default)
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
                _objCurrentUser = _userManager.Users.FirstOrDefault(x => x.DomainUserId == objUserId);
                return _objCurrentUser;
            }
            finally
            {
                _objGate.Release();
            }
        }

        public void Set(ApplicationUser? objUser)
        {
            _objCurrentUser = objUser;
            _objCurrentUserId = objUser?.DomainUserId;
        }

        public void Clear()
        {
            _objCurrentUser = null;
            _objCurrentUserId = null;
        }
    }
}