using CRM.Core.Opportunities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Opportunities.Configurations
{
    public sealed class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
    {
        public void Configure(EntityTypeBuilder<Opportunity> objEntity)
        {
            objEntity.ToTable("T_Opportunities");

            objEntity.HasKey(objOpportunity => objOpportunity.Id);

            objEntity.Property(objOpportunity => objOpportunity.Name)
                .HasMaxLength(250)
                .IsRequired();

            objEntity.Property(objOpportunity => objOpportunity.Description)
                .HasMaxLength(1000);

            objEntity.Property(objOpportunity => objOpportunity.Stage)
                .HasConversion<Int32>()
                .IsRequired();

            objEntity.Property(objOpportunity => objOpportunity.Value)
                .HasPrecision(18, 2);

            objEntity.Property(objOpportunity => objOpportunity.Source)
                .HasMaxLength(150);

            objEntity.Property(objOpportunity => objOpportunity.Notes)
                .HasMaxLength(4000);

            objEntity.HasOne(objOpportunity => objOpportunity.Entity)
                .WithOne()
                .HasForeignKey<Opportunity>(objOpportunity => objOpportunity.Id)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objOpportunity => objOpportunity.Company)
                .WithMany()
                .HasForeignKey(objOpportunity => objOpportunity.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objOpportunity => objOpportunity.Contact)
                .WithMany()
                .HasForeignKey(objOpportunity => objOpportunity.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}