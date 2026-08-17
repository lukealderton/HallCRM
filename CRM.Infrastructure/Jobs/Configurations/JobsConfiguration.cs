using CRM.Core.Jobs.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Jobs.Configurations
{
    public sealed class JobsConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> objEntity)
        {
            objEntity.ToTable("T_Jobs");

            objEntity.HasKey(objJob => objJob.Id);

            objEntity.Property(objJob => objJob.Name)
                .HasMaxLength(250)
                .IsRequired();

            objEntity.Property(objJob => objJob.Description)
                .HasMaxLength(1000);

            objEntity.Property(objJob => objJob.Stage)
                .HasConversion<Int32>()
                .IsRequired();

            objEntity.Property(objJob => objJob.Value)
                .HasPrecision(18, 2);

            objEntity.Property(objJob => objJob.Source)
                .HasMaxLength(150);

            objEntity.Property(objJob => objJob.Notes)
                .HasMaxLength(4000);

            objEntity.Property(
                    objJob =>
                        objJob.AddressLine1)
                .HasColumnName(
                    "jobAddressLine1")
                .HasMaxLength(
                    250);

            objEntity.Property(
                    objJob =>
                        objJob.AddressLine2)
                .HasColumnName(
                    "jobAddressLine2")
                .HasMaxLength(
                    250);

            objEntity.Property(
                    objJob =>
                        objJob.Town)
                .HasColumnName(
                    "jobTown")
                .HasMaxLength(
                    150);

            objEntity.Property(
                    objJob =>
                        objJob.County)
                .HasColumnName(
                    "jobCounty")
                .HasMaxLength(
                    150);

            objEntity.Property(
                    objJob =>
                        objJob.Postcode)
                .HasColumnName(
                    "jobPostcode")
                .HasMaxLength(
                    20);

            objEntity.Property(
                    objJob =>
                        objJob.SiteContactName)
                .HasColumnName(
                    "jobSiteContactName")
                .HasMaxLength(
                    200);

            objEntity.Property(
                    objJob =>
                        objJob.SiteContactPhone)
                .HasColumnName(
                    "jobSiteContactPhone")
                .HasMaxLength(
                    50);

            objEntity.Property(
                    objJob =>
                        objJob.AccessNotes)
                .HasColumnName(
                    "jobAccessNotes")
                .HasMaxLength(
                    2000);

            objEntity.Property(
                    objJob =>
                        objJob.AssignedUserId)
                .HasColumnName(
                    "jobAssignedUserId");

            objEntity.HasOne(objJob => objJob.Entity)
                .WithOne()
                .HasForeignKey<Job>(objJob => objJob.Id)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objJob => objJob.Company)
                .WithMany()
                .HasForeignKey(objJob => objJob.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objJob => objJob.Contact)
                .WithMany()
                .HasForeignKey(objJob => objJob.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}