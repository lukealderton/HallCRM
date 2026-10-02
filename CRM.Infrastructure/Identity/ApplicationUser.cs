using Microsoft.AspNetCore.Identity;

namespace CRM.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public ApplicationUser()
        {
            // IdentityUser<string> doesn't generate a key (only the non-generic IdentityUser does)
            Id = Guid.NewGuid().ToString();
            SecurityStamp = Guid.NewGuid().ToString();
        }

        public Guid DomainUserId { get; set; }

        public Boolean Enabled { get; set; } = true;
        public String Forename { get; set; } = "";
        public String Surname { get; set; } = "";
        
        public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedUtc { get; set; }
        public DateTimeOffset? LastLoginUtc { get; set; }
    }
}