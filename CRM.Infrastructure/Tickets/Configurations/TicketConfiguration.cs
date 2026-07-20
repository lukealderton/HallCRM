using CRM.Core.Tickets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Tickets.Configurations
{
    public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> objBuilder)
        {
            objBuilder.ToTable("T_Ticket");

            objBuilder.HasKey(objTicket => objTicket.Id);

            objBuilder.Property(objTicket => objTicket.Title)
                .HasMaxLength(250)
                .IsRequired();

            objBuilder.Property(objTicket => objTicket.Description)
                .HasMaxLength(4000);

            objBuilder.Property(objTicket => objTicket.InternalNotes)
                .HasMaxLength(4000);

            objBuilder.Property(objTicket => objTicket.EstimatedValue)
                .HasPrecision(18, 2);

            objBuilder.Property(objTicket => objTicket.QuotedValue)
                .HasPrecision(18, 2);

            objBuilder.Property(objTicket => objTicket.InvoiceValue)
                .HasPrecision(18, 2);

            objBuilder.HasOne(objTicket => objTicket.Entity)
                .WithOne()
                .HasForeignKey<Ticket>(objTicket => objTicket.Id)
                .OnDelete(DeleteBehavior.Cascade);

            objBuilder.HasOne(objTicket => objTicket.Company)
                .WithMany()
                .HasForeignKey(objTicket => objTicket.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            objBuilder.HasIndex(objTicket => objTicket.CompanyId);
            objBuilder.HasIndex(objTicket => objTicket.ContactId);
            objBuilder.HasIndex(objTicket => objTicket.AssignedToUserId);
            objBuilder.HasIndex(objTicket => objTicket.Status);
            objBuilder.HasIndex(objTicket => objTicket.Type);
            objBuilder.HasIndex(objTicket => objTicket.Priority);
            objBuilder.HasIndex(objTicket => objTicket.IsChargeable);
            objBuilder.HasIndex(objTicket => objTicket.IsInvoiced);
            objBuilder.HasIndex(objTicket => objTicket.DueUtc);
        }
    }
}
