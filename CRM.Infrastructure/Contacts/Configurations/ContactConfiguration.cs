using CRM.Core.Contacts.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Contacts.Configurations
{
    public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> objEntity)
        {
            objEntity.ToTable("T_Contacts");

            objEntity.HasKey(objContact => objContact.Id);

            objEntity.Property(objContact => objContact.Title)
                .HasMaxLength(50);

            objEntity.Property(objContact => objContact.FirstName)
                .HasMaxLength(100);

            objEntity.Property(objContact => objContact.LastName)
                .HasMaxLength(100);

            objEntity.Property(objContact => objContact.JobTitle)
                .HasMaxLength(150);

            objEntity.Property(objContact => objContact.Department)
                .HasMaxLength(150);

            objEntity.Property(objContact => objContact.PrimaryEmail)
                .HasMaxLength(256);

            objEntity.Property(objContact => objContact.PrimaryPhone)
                .HasMaxLength(50);

            objEntity.Property(objContact => objContact.MobilePhone)
                .HasMaxLength(50);

            objEntity.Property(objContact => objContact.LinkedInUrl)
                .HasMaxLength(500);

            objEntity.HasOne(objContact => objContact.Entity)
                .WithOne()
                .HasForeignKey<Contact>(objContact => objContact.Id)
                .OnDelete(DeleteBehavior.Restrict);

            objEntity.HasOne(objContact => objContact.Company)
                .WithMany()
                .HasForeignKey(objContact => objContact.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}