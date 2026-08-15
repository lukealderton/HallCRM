using CRM.Core.Activities.Domain;
using CRM.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Activities.Configurations
{
    public sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> objEntity)
        {
            objEntity.ToTable("T_Activities");

            objEntity.HasKey(objActivity => objActivity.Id);

            objEntity.Property(objActivity => objActivity.Type)
                .HasConversion<Int32>()
                .IsRequired();

            objEntity.Property(objActivity => objActivity.Subject)
                .HasMaxLength(250)
                .IsRequired();

            objEntity.Property(objActivity => objActivity.Description)
                .HasMaxLength(4000);

            objEntity.Property(objActivity => objActivity.DueUtc);

            objEntity.Property(objActivity => objActivity.CompletedUtc);

            objEntity.Property(objActivity => objActivity.AssignedUserId);

            objEntity.HasOne(objActivity => objActivity.Entity)
                .WithOne()
                .HasForeignKey<Activity>(objActivity => objActivity.Id)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objActivity => objActivity.Company)
                .WithMany()
                .HasForeignKey(objActivity => objActivity.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objActivity => objActivity.Contact)
                .WithMany()
                .HasForeignKey(objActivity => objActivity.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objActivity => objActivity.Job)
                .WithMany()
                .HasForeignKey(objActivity => objActivity.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(objActivity => objActivity.AssignedUserId)
                .HasPrincipalKey(objUser => objUser.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasIndex(objActivity => objActivity.Type);
            objEntity.HasIndex(objActivity => objActivity.CompanyId);
            objEntity.HasIndex(objActivity => objActivity.ContactId);
            objEntity.HasIndex(objActivity => objActivity.JobId);
            objEntity.HasIndex(objActivity => objActivity.AssignedUserId);
            objEntity.HasIndex(objActivity => objActivity.DueUtc);
            objEntity.HasIndex(objActivity => objActivity.CompletedUtc);

            objEntity.HasIndex(objActivity => new
            {
                objActivity.CompletedUtc,
                objActivity.DueUtc
            });
        }
    }
}