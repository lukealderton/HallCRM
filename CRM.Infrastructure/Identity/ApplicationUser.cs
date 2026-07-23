using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public Guid DomainUserId { get; set; }
        //public Core.Users.Domain.UserProfile? Profile { get; set; }

        public Boolean Enabled { get; set; }
        public String Forename { get; set; } = "";
        public String Surname { get; set; } = "";
        
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset? UpdatedUtc { get; set; }
        public DateTimeOffset? LastLoginUtc { get; set; }
    }
}