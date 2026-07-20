using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> objEntity)
        {
            objEntity.ToTable("T_UserRole");

            objEntity.Property(x => x.UserId)
                .HasColumnName("urlUserId");

            objEntity.Property(x => x.RoleId)
                .HasColumnName("urlRoleId");
        }
    }
}