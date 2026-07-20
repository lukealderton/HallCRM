using CRM.Core.Companies.Domain;
using CRM.Infrastructure.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Companies.Configurations
{
    public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> objEntity)
        {
            objEntity.ToTable("T_Company");

            objEntity.ConfigureEntityRecord("cmpId");

            objEntity.Property(x => x.Name)
                .HasColumnName("cmpName")
                .HasMaxLength(250)
                .IsRequired();

            objEntity.Property(x => x.TradingName)
                .HasColumnName("cmpTradingName")
                .HasMaxLength(250);

            objEntity.Property(x => x.Website)
                .HasColumnName("cmpWebsite")
                .HasMaxLength(500);

            objEntity.Property(x => x.PrimaryEmail)
                .HasColumnName("cmpPrimaryEmail")
                .HasMaxLength(320);

            objEntity.Property(x => x.PrimaryPhone)
                .HasColumnName("cmpPrimaryPhone")
                .HasMaxLength(50);

            objEntity.Property(x => x.AddressLine1)
                .HasColumnName("cmpAdrLine1")
                .HasMaxLength(200);

            objEntity.Property(x => x.AddressLine2)
                .HasColumnName("cmpAdrLine2")
                .HasMaxLength(200);

            objEntity.Property(x => x.Town)
                .HasColumnName("cmpAdrTown")
                .HasMaxLength(100);

            objEntity.Property(x => x.County)
                .HasColumnName("cmpAdrCounty")
                .HasMaxLength(100);

            objEntity.Property(x => x.Postcode)
                .HasColumnName("cmpAdrPostcode")
                .HasMaxLength(20);

            objEntity.Property(x => x.Country)
                .HasColumnName("cmpAdrCountry")
                .HasMaxLength(100);

            objEntity.Property(x => x.CompanyNumber)
                .HasColumnName("cmpCompanyNumber")
                .HasMaxLength(100);

            objEntity.Property(x => x.CharityNumber)
                .HasColumnName("cmpCharityNumber")
                .HasMaxLength(100);

            objEntity.Property(x => x.VatNumber)
                .HasColumnName("cmpVatNumber")
                .HasMaxLength(100);

            objEntity.Property(x => x.Industry)
                .HasColumnName("cmpIndustry")
                .HasMaxLength(150);

            objEntity.Property(x => x.Notes)
                .HasColumnName("cmpNotes");

            objEntity.HasIndex(x => x.Name);
            objEntity.HasIndex(x => x.PrimaryEmail);
            objEntity.HasIndex(x => x.PrimaryPhone);
            objEntity.HasIndex(x => x.CompanyNumber);
        }
    }
}