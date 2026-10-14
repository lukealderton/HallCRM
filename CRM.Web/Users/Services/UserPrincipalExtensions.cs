using System.Security.Claims;
using CRM.Core.Users.Abstraction.Services;
using CRM.Core.Users.Domain;

namespace CRM.Web.Users.Services;

public static class UserPrincipalExtensions
{
    public static async Task<User?> GetCrmUserAsync(
        this ClaimsPrincipal objPrincipal, IUserService objUsers, CancellationToken objToken = default)
    {
        if (objPrincipal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // NameIdentifier is the login account ID; audit and media access use the domain ID.
        String? strIdentityId = objPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        User? objUser = null;
        if (!String.IsNullOrWhiteSpace(strIdentityId))
        {
            objUser = await objUsers.GetUserByIdentityIdAsync(strIdentityId, objToken);
        }
        else if (Guid.TryParse(objPrincipal.FindFirst("Id")?.Value, out Guid objDomainId)
            && objDomainId != Guid.Empty)
        {
            // Older principals may carry only the custom domain-user claim.
            objUser = await objUsers.GetUserAsync(objDomainId, objToken);
        }

        return objUser?.Id == Guid.Empty ? null : objUser;
    }
}
