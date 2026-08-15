using CRM.Core.Activities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Activities.Configurations
{
    public sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> objActivity)
        {
            objActivity.ToTable("T_Activity");

            objActivity.HasKey(x => x.Id);

            objActivity.Property(x => x.Id)
                .HasColumnName("actId");

            objActivity.Property(x => x.CompanyId)
                .HasColumnName("actCompanyId");

            objActivity.Property(x => x.ContactId)
                .HasColumnName("actContactId");

            objActivity.Property(x => x.JobId)
                .HasColumnName("actJobId");

            objActivity.Property(x => x.AssignedUserId)
                .HasColumnName("actAssignedUserId");

            objActivity.Property(x => x.Type)
                .HasColumnName("actType")
                .IsRequired();

            objActivity.Property(x => x.Subject)
                .HasColumnName("actSubject")
                .HasMaxLength(250)
                .IsRequired();

            objActivity.Property(x => x.Description)
                .HasColumnName("actDescription")
                .HasMaxLength(4000);

            objActivity.Property(x => x.DueUtc)
                .HasColumnName("actDueUtc");

            objActivity.Property(x => x.CompletedUtc)
                .HasColumnName("actCompletedUtc");

            objActivity.HasOne(x => x.Entity)
                .WithOne()
                .HasForeignKey<Activity>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            objActivity.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            objActivity.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            objActivity.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            objActivity.HasIndex(x => x.CompanyId);
            objActivity.HasIndex(x => x.ContactId);
            objActivity.HasIndex(x => x.JobId);
            objActivity.HasIndex(x => x.AssignedUserId);
            objActivity.HasIndex(x => x.Type);
            objActivity.HasIndex(x => x.DueUtc);
            objActivity.HasIndex(x => x.CompletedUtc);

            objActivity.HasIndex(x => new
            {
                x.CompletedUtc,
                x.DueUtc
            });
        }
    }
}