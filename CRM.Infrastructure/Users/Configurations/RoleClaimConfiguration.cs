using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> objEntity)
        {
            objEntity.ToTable("T_RoleClaim");

            objEntity.Property(x => x.Id)
                .HasColumnName("rclId");

            objEntity.Property(x => x.RoleId)
                .HasColumnName("rclRoleId");

            objEntity.Property(x => x.ClaimType)
                .HasColumnName("rclClaimType");

            objEntity.Property(x => x.ClaimValue)
                .HasColumnName("rclClaimValue");
        }
    }
}