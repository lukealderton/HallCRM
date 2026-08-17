using CRM.Core.Payments.Domain;
using CRM.Infrastructure.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Payments.Configurations
{
    public sealed class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(
            EntityTypeBuilder<Payment> objBuilder)
        {
            objBuilder.ToTable(
                "T_Payment");

            objBuilder.ConfigureEntityRecord(
                "payId");

            objBuilder.Property(
                    objPayment =>
                        objPayment.InvoiceId)
                .HasColumnName(
                    "payInvoiceId")
                .IsRequired();

            objBuilder.Property(
                    objPayment =>
                        objPayment.Amount)
                .HasColumnName(
                    "payAmount")
                .HasPrecision(
                    18,
                    2)
                .IsRequired();

            objBuilder.Property(
                    objPayment =>
                        objPayment.PaymentDateUtc)
                .HasColumnName(
                    "payPaymentDateUtc")
                .IsRequired();

            objBuilder.Property(
                    objPayment =>
                        objPayment.Method)
                .HasColumnName(
                    "payMethod")
                .IsRequired();

            objBuilder.Property(
                    objPayment =>
                        objPayment.Reference)
                .HasColumnName(
                    "payReference")
                .HasMaxLength(
                    200);

            objBuilder.Property(
                    objPayment =>
                        objPayment.Notes)
                .HasColumnName(
                    "payNotes")
                .HasMaxLength(
                    2000);

            objBuilder.HasIndex(
                objPayment =>
                    objPayment.InvoiceId);

            objBuilder.HasIndex(
                objPayment =>
                    objPayment.PaymentDateUtc);

            objBuilder.HasOne(
                    objPayment =>
                        objPayment.Invoice)
                .WithMany(
                    objInvoice =>
                        objInvoice.Payments)
                .HasForeignKey(
                    objPayment =>
                        objPayment.InvoiceId)
                .OnDelete(
                    DeleteBehavior.NoAction);
        }
    }
}