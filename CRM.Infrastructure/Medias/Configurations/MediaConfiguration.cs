using CRM.Core.Jobs.Domain;
using CRM.Core.Medias.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Medias.Configurations;

public sealed class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> objBuilder)
    {
        objBuilder.ToTable("T_Media");
        objBuilder.HasKey(objMedia => objMedia.Id);
        objBuilder.Property(objMedia => objMedia.FileName).HasMaxLength(255).IsRequired();
        objBuilder.Property(objMedia => objMedia.Key).HasMaxLength(500).IsRequired();
        objBuilder.Property(objMedia => objMedia.ContentType).HasMaxLength(100).IsRequired();
        objBuilder.Ignore(objMedia => objMedia.IsImage);
        objBuilder.Ignore(objMedia => objMedia.IsVideo);
        objBuilder.Ignore(objMedia => objMedia.Url);
        objBuilder.HasIndex(objMedia => new { objMedia.JobId, objMedia.DeletedUtc, objMedia.CreatedUtc });
        objBuilder.HasIndex(objMedia => objMedia.Key).IsUnique();
        objBuilder.HasOne<Job>().WithMany().HasForeignKey(objMedia => objMedia.JobId).OnDelete(DeleteBehavior.Restrict);
    }
}
