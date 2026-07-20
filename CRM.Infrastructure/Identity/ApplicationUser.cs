using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public Guid DomainUserId { get; set; }
        public Core.Users.Domain.UserProfile? Profile { get; set; }
        public DateTimeOffset? LastLoginUtc { get; set; }
    }
}