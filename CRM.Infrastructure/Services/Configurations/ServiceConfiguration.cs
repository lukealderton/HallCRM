using CRM.Core.Services.Domain;
using CRM.Infrastructure.Entities.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Services.Configurations
{
    public sealed class ServiceConfiguration :
        IEntityTypeConfiguration<Service>
    {
        public void Configure(
            EntityTypeBuilder<Service> objBuilder)
        {
            objBuilder.ToTable(
                "T_Service");

            objBuilder.ConfigureEntityRecord(
                "svcId");

            objBuilder.Property(
                    objService =>
                        objService.Name)
                .HasColumnName(
                    "svcName")
                .HasMaxLength(
                    200)
                .IsRequired();

            objBuilder.Property(
                    objService =>
                        objService.Description)
                .HasColumnName(
                    "svcDescription")
                .HasMaxLength(
                    2000);

            objBuilder.Property(
                    objService =>
                        objService.DefaultPrice)
                .HasColumnName(
                    "svcDefaultPrice")
                .HasPrecision(
                    18,
                    2);

            objBuilder.Property(
                    objService =>
                        objService.Notes)
                .HasColumnName(
                    "svcNotes");
        }
    }
}