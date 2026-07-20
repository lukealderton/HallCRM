using CRM.Core.Entities.Domain;
using CRM.Core.Users.Domain;
using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Entities.Configurations
{
    public sealed class EntityConfiguration : IEntityTypeConfiguration<CrmEntity>
    {
        public void Configure(EntityTypeBuilder<CrmEntity> objEntity)
        {
            objEntity.ToTable("T_Entity");

            objEntity.HasKey(x => x.Id);

            objEntity.Property(x => x.Id)
                .HasColumnName("entId");

            objEntity.Property(x => x.EntityTypeId)
                .HasColumnName("entTypeId")
                .IsRequired();

            objEntity.Property(x => x.DisplayName)
                .HasColumnName("entDisplayName")
                .HasMaxLength(300)
                .IsRequired();

            objEntity.Property(x => x.OwnerUserId)
                .HasColumnName("entOwnerUserId");

            objEntity.Property(x => x.CreatedUtc)
                .HasColumnName("entCreatedUtc")
                .IsRequired();

            objEntity.Property(x => x.CreatedByUserId)
                .HasColumnName("entCreatedByUserId");

            objEntity.Property(x => x.UpdatedUtc)
                .HasColumnName("entUpdatedUtc");

            objEntity.Property(x => x.UpdatedByUserId)
                .HasColumnName("entUpdatedByUserId");

            objEntity.Property(x => x.ArchivedUtc)
                .HasColumnName("entArchivedUtc");

            objEntity.Property(x => x.ArchivedByUserId)
                .HasColumnName("entArchivedByUserId");

            objEntity.Property(x => x.DeletedUtc)
                .HasColumnName("entDeletedUtc");

            objEntity.Property(x => x.DeletedByUserId)
                .HasColumnName("entDeletedByUserId");

            objEntity.Property(x => x.RowVersion)
                .HasColumnName("entRowVersion")
                .IsRowVersion();

            objEntity.HasOne(x => x.EntityType)
                .WithMany(x => x.Entities)
                .HasForeignKey(x => x.EntityTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.OwnerUserId)
                .HasPrincipalKey(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .HasPrincipalKey(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UpdatedByUserId)
                .HasPrincipalKey(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ArchivedByUserId)
                .HasPrincipalKey(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.DeletedByUserId)
                .HasPrincipalKey(x => x.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasIndex(x => x.EntityTypeId);
            objEntity.HasIndex(x => x.DisplayName);
            objEntity.HasIndex(x => x.OwnerUserId);
            objEntity.HasIndex(x => x.CreatedUtc);
            objEntity.HasIndex(x => x.ArchivedUtc);
            objEntity.HasIndex(x => x.DeletedUtc);

            objEntity.HasIndex(x => new
            {
                x.EntityTypeId,
                x.DeletedUtc
            });
        }
    }
}