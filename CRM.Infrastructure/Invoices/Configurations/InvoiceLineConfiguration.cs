using CRM.Core.Invoices.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Invoices.Configurations
{
    public sealed class InvoiceLineConfiguration
        : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(
            EntityTypeBuilder<InvoiceLine> objBuilder)
        {
            objBuilder.ToTable(
                "T_InvoiceLine");

            objBuilder.HasKey(
                objLine =>
                    objLine.Id);

            objBuilder.Property(
                    objLine =>
                        objLine.Id)
                .HasColumnName(
                    "inlId");

            objBuilder.Property(
                    objLine =>
                        objLine.InvoiceId)
                .HasColumnName(
                    "inlInvoiceId")
                .IsRequired();

            objBuilder.Property(
                    objLine =>
                        objLine.ServiceId)
                .HasColumnName(
                    "inlServiceId");

            objBuilder.Property(
                    objLine =>
                        objLine.SortOrder)
                .HasColumnName(
                    "inlSortOrder")
                .IsRequired();

            objBuilder.Property(
                    objLine =>
                        objLine.Description)
                .HasColumnName(
                    "inlDescription")
                .HasMaxLength(
                    1000)
                .IsRequired();

            objBuilder.Property(
                    objLine =>
                        objLine.Quantity)
                .HasColumnName(
                    "inlQuantity")
                .HasPrecision(
                    10,
                    2)
                .IsRequired();

            objBuilder.Property(
                    objLine =>
                        objLine.UnitPrice)
                .HasColumnName(
                    "inlUnitPrice")
                .HasPrecision(
                    18,
                    2)
                .IsRequired();

            objBuilder.HasIndex(
                objLine =>
                    objLine.InvoiceId);

            objBuilder.HasOne(
                    objLine =>
                        objLine.Invoice)
                .WithMany(
                    objInvoice =>
                        objInvoice.Lines)
                .HasForeignKey(
                    objLine =>
                        objLine.InvoiceId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            objBuilder.HasOne(
                    objLine =>
                        objLine.Service)
                .WithMany()
                .HasForeignKey(
                    objLine =>
                        objLine.ServiceId)
                .OnDelete(
                    DeleteBehavior.NoAction);
        }
    }
}