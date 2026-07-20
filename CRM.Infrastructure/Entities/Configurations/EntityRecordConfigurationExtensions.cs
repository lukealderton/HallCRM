using CRM.Core.Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Entities.Configurations
{
    public static class EntityRecordConfigurationExtensions
    {
        public static void ConfigureEntityRecord<TRecord>(
            this EntityTypeBuilder<TRecord> objEntity,
            String strIdColumnName)
            where TRecord : CrmEntityRecord
        {
            objEntity.HasKey(x => x.Id);

            objEntity.Property(x => x.Id)
                .HasColumnName(strIdColumnName);

            objEntity.HasOne(x => x.Entity)
                .WithOne()
                .HasForeignKey<TRecord>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            objEntity.Navigation(x => x.Entity)
                .IsRequired();

            objEntity.Ignore(x => x.IsArchived);
            objEntity.Ignore(x => x.IsDeleted);
            objEntity.Ignore(x => x.IsActive);
        }
    }
}