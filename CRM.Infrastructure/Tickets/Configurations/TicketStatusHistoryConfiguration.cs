using CRM.Core.Tickets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Tickets.Configurations
{
    public sealed class TicketStatusHistoryConfiguration : IEntityTypeConfiguration<TicketStatusHistory>
    {
        public void Configure(EntityTypeBuilder<TicketStatusHistory> objBuilder)
        {
            objBuilder.ToTable("T_TicketStatusHistory");

            objBuilder.HasKey(objHistory => objHistory.Id);

            objBuilder.HasOne(objHistory => objHistory.Ticket)
                .WithMany(objTicket => objTicket.StatusHistory)
                .HasForeignKey(objHistory => objHistory.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            objBuilder.HasIndex(objHistory => objHistory.TicketId);
            objBuilder.HasIndex(objHistory => objHistory.ChangedUtc);
        }
    }
}
