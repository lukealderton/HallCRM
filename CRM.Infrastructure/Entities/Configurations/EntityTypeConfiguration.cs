using CRM.Core.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Entities.Configurations
{
    public sealed class EntityTypeConfiguration : IEntityTypeConfiguration<CrmEntityType>
    {
        public void Configure(EntityTypeBuilder<CrmEntityType> objEntity)
        {
            objEntity.ToTable("T_EntityType");

            objEntity.HasKey(x => x.Id);

            objEntity.Property(x => x.Id)
                .HasColumnName("etyId");

            objEntity.Property(x => x.Name)
                .HasColumnName("etyName")
                .HasMaxLength(100)
                .IsRequired();

            objEntity.Property(x => x.Alias)
                .HasColumnName("etyAlias")
                .HasMaxLength(100)
                .IsRequired();

            objEntity.Property(x => x.IsSystem)
                .HasColumnName("etySystem")
                .IsRequired();

            objEntity.Property(x => x.IsCustom)
                .HasColumnName("etyCustom")
                .IsRequired();

            objEntity.Property(x => x.CreatedUtc)
                .HasColumnName("etyCreatedUtc")
                .IsRequired();

            objEntity.HasIndex(x => x.Alias)
                .IsUnique();

            DateTime objSeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            objEntity.HasData(
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.Company,
                    Name = "Company",
                    Alias = "company",
                    IsSystem = true,
                    IsCustom = false,
                    CreatedUtc = objSeedDate
                },
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.Contact,
                    Name = "Contact",
                    Alias = "contact",
                    IsSystem = true,
                    IsCustom = false,
                    CreatedUtc = objSeedDate
                },
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.Opportunity,
                    Name = "Opportunity",
                    Alias = "opportunity",
                    IsSystem = true,
                    IsCustom = false,
                    CreatedUtc = objSeedDate
                },
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.Ticket,
                    Name = "Ticket",
                    Alias = "ticket",
                    IsSystem = true,
                    IsCustom = false,
                    CreatedUtc = objSeedDate
                },
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.Activity,
                    Name = "Activity",
                    Alias = "activity",
                    IsSystem = true,
                    IsCustom = false,
                    CreatedUtc = objSeedDate
                },
                new CrmEntityType
                {
                    Id = (Int32)PredefinedEntityType.CustomRecord,
                    Name = "Custom Record",
                    Alias = "custom-record",
                    IsSystem = true,
                    IsCustom = true,
                    CreatedUtc = objSeedDate
                }
            );
        }
    }
}