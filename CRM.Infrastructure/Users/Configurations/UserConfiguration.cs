using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> objEntity)
        {
            objEntity.ToTable("T_User");

            objEntity.Property(x => x.Id)
                .HasColumnName("usrId");

            objEntity.Property(x => x.UserName)
                .HasColumnName("usrUsername");

            objEntity.Property(x => x.NormalizedUserName)
                .HasColumnName("usrUsernameNorm");

            objEntity.Property(x => x.Email)
                .HasColumnName("usrEmail");

            objEntity.Property(x => x.NormalizedEmail)
                .HasColumnName("usrEmailNorm");

            objEntity.Property(x => x.EmailConfirmed)
                .HasColumnName("usrEmailConfirmed");

            objEntity.Property(x => x.PasswordHash)
                .HasColumnName("usrPasswordHash");

            objEntity.Property(x => x.SecurityStamp)
                .HasColumnName("usrSecurityStamp");

            objEntity.Property(x => x.ConcurrencyStamp)
                .HasColumnName("usrConcurrencyStamp");

            objEntity.Property(x => x.PhoneNumber)
                .HasColumnName("usrPhone");

            objEntity.Property(x => x.PhoneNumberConfirmed)
                .HasColumnName("usrPhoneConfirmed");

            objEntity.Property(x => x.TwoFactorEnabled)
                .HasColumnName("usrTwoFactor");

            objEntity.Property(x => x.LockoutEnd)
                .HasColumnName("usrLockoutEnd");

            objEntity.Property(x => x.LockoutEnabled)
                .HasColumnName("usrLockoutEnabled");

            objEntity.Property(x => x.AccessFailedCount)
                .HasColumnName("usrAccessFailed");

            objEntity.Property(x => x.LastLoginUtc)
                .HasColumnName("usrLastLoginUtc");

            objEntity.Property(x => x.CreatedUtc)
                .HasColumnName("usrCreatedUtc");

            objEntity.Property(x => x.UpdatedUtc)
                .HasColumnName("usrUpdatedUtc");

            objEntity.Property(x => x.DomainUserId)
                .HasColumnName("usrDomainUserId")
                .IsRequired();

            objEntity.HasAlternateKey(x => x.DomainUserId);

            objEntity.HasIndex(x => x.DomainUserId)
                .IsUnique();
        }
    }
}