using CRM.Core.Invoices.Domain;
using CRM.Infrastructure.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Invoices.Configurations
{
    public sealed class InvoiceConfiguration
        : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(
            EntityTypeBuilder<Invoice> objBuilder)
        {
            objBuilder.ToTable(
                "T_Invoice");

            objBuilder.ConfigureEntityRecord(
                "invId");

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.JobId)
                .HasColumnName(
                    "invJobId")
                .IsRequired();

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.CompanyId)
                .HasColumnName(
                    "invCompanyId");

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.ContactId)
                .HasColumnName(
                    "invContactId");

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.InvoiceNumber)
                .HasColumnName(
                    "invNumber")
                .HasMaxLength(
                    50)
                .IsRequired();

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.Status)
                .HasColumnName(
                    "invStatus")
                .IsRequired();

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.IssueDateUtc)
                .HasColumnName(
                    "invIssueDateUtc");

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.DueDateUtc)
                .HasColumnName(
                    "invDueDateUtc");

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.CustomerName)
                .HasColumnName(
                    "invCustomerName")
                .HasMaxLength(
                    300);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.AddressLine1)
                .HasColumnName(
                    "invAddressLine1")
                .HasMaxLength(
                    250);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.AddressLine2)
                .HasColumnName(
                    "invAddressLine2")
                .HasMaxLength(
                    250);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.Town)
                .HasColumnName(
                    "invTown")
                .HasMaxLength(
                    150);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.County)
                .HasColumnName(
                    "invCounty")
                .HasMaxLength(
                    150);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.Postcode)
                .HasColumnName(
                    "invPostcode")
                .HasMaxLength(
                    20);

            objBuilder.Property(
                    objInvoice =>
                        objInvoice.Notes)
                .HasColumnName(
                    "invNotes")
                .HasMaxLength(
                    4000);

            objBuilder.HasIndex(
                    objInvoice =>
                        objInvoice.InvoiceNumber)
                .IsUnique();

            objBuilder.HasIndex(
                objInvoice =>
                    objInvoice.JobId);

            objBuilder.HasIndex(
                objInvoice =>
                    objInvoice.CompanyId);

            objBuilder.HasIndex(
                objInvoice =>
                    objInvoice.Status);

            objBuilder.HasOne(
                    objInvoice =>
                        objInvoice.Job)
                .WithMany(
                    objJob =>
                        objJob.Invoices)
                .HasForeignKey(
                    objInvoice =>
                        objInvoice.JobId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            objBuilder.HasOne(
                    objInvoice =>
                        objInvoice.Company)
                .WithMany()
                .HasForeignKey(
                    objInvoice =>
                        objInvoice.CompanyId)
                .OnDelete(
                    DeleteBehavior.Restrict);

            objBuilder.HasOne(
                    objInvoice =>
                        objInvoice.Contact)
                .WithMany()
                .HasForeignKey(
                    objInvoice =>
                        objInvoice.ContactId)
                .OnDelete(
                    DeleteBehavior.Restrict);
        }
    }
}