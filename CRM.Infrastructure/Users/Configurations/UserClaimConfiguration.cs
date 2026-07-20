using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> objEntity)
        {
            objEntity.ToTable("T_UserClaim");

            objEntity.Property(x => x.Id)
                .HasColumnName("ucmId");

            objEntity.Property(x => x.UserId)
                .HasColumnName("ucmUserId");

            objEntity.Property(x => x.ClaimType)
                .HasColumnName("ucmClaimType");

            objEntity.Property(x => x.ClaimValue)
                .HasColumnName("ucmClaimValue");
        }
    }
}