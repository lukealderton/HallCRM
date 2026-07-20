using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> objEntity)
        {
            objEntity.ToTable("T_UserLogin");

            objEntity.Property(x => x.UserId)
                .HasColumnName("lgnUserId");

            objEntity.Property(x => x.LoginProvider)
                .HasColumnName("lgnProvider");

            objEntity.Property(x => x.ProviderKey)
                .HasColumnName("lgnProviderKey");

            objEntity.Property(x => x.ProviderDisplayName)
                .HasColumnName("lgnProviderDisplayName");
        }
    }
}