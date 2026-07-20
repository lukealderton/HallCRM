using CRM.Core.Tickets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Tickets.Configurations
{
    public sealed class TicketTimeEntryConfiguration : IEntityTypeConfiguration<TicketTimeEntry>
    {
        public void Configure(EntityTypeBuilder<TicketTimeEntry> objBuilder)
        {
            objBuilder.ToTable("T_TicketTimeEntry");

            objBuilder.HasKey(objTimeEntry => objTimeEntry.Id);

            objBuilder.Property(objTimeEntry => objTimeEntry.Notes)
                .HasMaxLength(2000);

            objBuilder.HasOne(objTimeEntry => objTimeEntry.Ticket)
                .WithMany(objTicket => objTicket.TimeEntries)
                .HasForeignKey(objTimeEntry => objTimeEntry.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            objBuilder.HasIndex(objTimeEntry => objTimeEntry.TicketId);
            objBuilder.HasIndex(objTimeEntry => objTimeEntry.UserId);
            objBuilder.HasIndex(objTimeEntry => objTimeEntry.CreatedUtc);
            objBuilder.HasIndex(objTimeEntry => objTimeEntry.IsInvoiced);
        }
    }
}
