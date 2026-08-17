using CRM.Core.Jobs.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Jobs.Configurations
{
    public sealed class JobServiceLinkConfiguration :
        IEntityTypeConfiguration<JobServiceLink>
    {
        public void Configure(
            EntityTypeBuilder<JobServiceLink> objBuilder)
        {
            objBuilder.ToTable(
                "T_JobService");

            objBuilder.HasKey(
                objLink =>
                    new
                    {
                        objLink.JobId,
                        objLink.ServiceId
                    });

            objBuilder.Property(
                    objLink =>
                        objLink.JobId)
                .HasColumnName(
                    "jbsJobId");

            objBuilder.Property(
                    objLink =>
                        objLink.ServiceId)
                .HasColumnName(
                    "jbsServiceId");

            objBuilder.HasOne(
                    objLink =>
                        objLink.Job)
                .WithMany(
                    objJob =>
                        objJob.ServiceLinks)
                .HasForeignKey(
                    objLink =>
                        objLink.JobId)
                .OnDelete(
                    DeleteBehavior.Cascade);

            objBuilder.HasOne(
                    objLink =>
                        objLink.Service)
                .WithMany(
                    objService =>
                        objService.JobLinks)
                .HasForeignKey(
                    objLink =>
                        objLink.ServiceId)
                .OnDelete(
                    DeleteBehavior.Restrict);
        }
    }
}