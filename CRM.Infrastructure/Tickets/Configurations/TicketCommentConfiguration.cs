using CRM.Core.Tickets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Tickets.Configurations
{
    public sealed class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> objBuilder)
        {
            objBuilder.ToTable("T_TicketComment");

            objBuilder.HasKey(objComment => objComment.Id);

            objBuilder.Property(objComment => objComment.Text)
                .HasMaxLength(4000)
                .IsRequired();

            objBuilder.HasOne(objComment => objComment.Ticket)
                .WithMany(objTicket => objTicket.Comments)
                .HasForeignKey(objComment => objComment.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            objBuilder.HasIndex(objComment => objComment.TicketId);
            objBuilder.HasIndex(objComment => objComment.CreatedUtc);
        }
    }
}
