using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> objEntity)
        {
            objEntity.ToTable("T_UserToken");

            objEntity.Property(x => x.UserId)
                .HasColumnName("utnUserId");

            objEntity.Property(x => x.LoginProvider)
                .HasColumnName("utnLoginProvider");

            objEntity.Property(x => x.Name)
                .HasColumnName("utnName");

            objEntity.Property(x => x.Value)
                .HasColumnName("utnValue");
        }
    }
}