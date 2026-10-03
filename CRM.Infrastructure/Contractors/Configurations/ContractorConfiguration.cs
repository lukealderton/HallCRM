using CRM.Core.Contractors.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Contractors.Configurations;

public sealed class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> objBuilder)
    {
        objBuilder.ToTable("T_Contractor");
        objBuilder.HasKey(objContractor => objContractor.Id);
        objBuilder.Property(objContractor => objContractor.Name).HasMaxLength(200).IsRequired();
        objBuilder.Property(objContractor => objContractor.CompanyName).HasMaxLength(200);
        objBuilder.Property(objContractor => objContractor.Email).HasMaxLength(254);
        objBuilder.Property(objContractor => objContractor.Phone).HasMaxLength(50);
        objBuilder.Property(objContractor => objContractor.Notes).HasMaxLength(4000);
        objBuilder.Ignore(objContractor => objContractor.DisplayName);
        objBuilder.HasIndex(objContractor => new { objContractor.Enabled, objContractor.Name });
    }
}
