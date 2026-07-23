using CRM.Core.Users.Domain;
using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> objEntity)
        {
            objEntity.ToTable("T_UserProfile");

            objEntity.HasKey(x => x.Id);

            objEntity.Property(x => x.Id)
                .HasColumnName("uspId");

            objEntity.Property(x => x.Forename)
                .HasColumnName("uspForename");

            objEntity.Property(x => x.Surname)
                .HasColumnName("uspSurname");

            objEntity.Property(x => x.Mobile)
                .HasColumnName("uspMobile");

            objEntity.Property(x => x.AddressLine1)
                .HasColumnName("uspAdrLine1");

            objEntity.Property(x => x.AddressLine2)
                .HasColumnName("uspAdrLine2");

            objEntity.Property(x => x.Town)
                .HasColumnName("uspAdrTown");

            objEntity.Property(x => x.County)
                .HasColumnName("uspAdrCounty");

            objEntity.Property(x => x.Postcode)
                .HasColumnName("uspAdrPostcode");

            objEntity.Property(x => x.Country)
                .HasColumnName("uspAdrCountry");

            objEntity.Property(x => x.StripeCustomerId)
                .HasColumnName("uspStripeCustomerId");

            objEntity.Property(x => x.CreatedUtc)
                .HasColumnName("uspCreatedUtc");

            objEntity.Property(x => x.UpdatedUtc)
                .HasColumnName("uspUpdatedUtc");

            //objEntity.HasOne<ApplicationUser>()
            //    .WithOne(x => x.Profile)
            //    .HasForeignKey<UserProfile>(x => x.Id)
            //    .HasPrincipalKey<ApplicationUser>(x => x.DomainUserId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}