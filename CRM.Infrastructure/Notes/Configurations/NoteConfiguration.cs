using CRM.Core.Notes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Notes.Configurations
{
    public sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> objEntity)
        {
            objEntity.ToTable("T_Note");

            objEntity.HasKey(x => x.Id);

            objEntity.Property(x => x.Body)
                .IsRequired()
                .HasMaxLength(4000);

            objEntity.Property(x => x.CreatedUtc)
                .IsRequired();

            objEntity.HasIndex(x => new { x.EntityId, x.CreatedUtc });

            objEntity.HasOne(x => x.Entity)
                .WithMany()
                .HasForeignKey(x => x.EntityId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}