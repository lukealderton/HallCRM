using CRM.Core.Logging.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Logging.Configurations
{
    public sealed class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> objBuilder)
        {
            objBuilder.ToTable("T_Log");

            objBuilder.HasKey(objLog => objLog.Id);

            objBuilder.Property(objLog => objLog.Id)
                .HasColumnName("logId")
                .ValueGeneratedNever();

            objBuilder.Property(objLog => objLog.Timestamp)
                .HasColumnName("logTimestamp")
                .HasColumnType("datetime2")
                .IsRequired();

            objBuilder.Property(objLog => objLog.LogType)
                .HasColumnName("logType")
                .HasConversion<Int32>()
                .IsRequired();

            objBuilder.Property(objLog => objLog.LogArea)
                .HasColumnName("logArea")
                .HasConversion<Int32>()
                .IsRequired();

            objBuilder.Property(objLog => objLog.MemberId)
                .HasColumnName("logMemberId");

            objBuilder.Property(objLog => objLog.RelId)
                .HasColumnName("logRelId");

            objBuilder.Property(objLog => objLog.RelType)
                .HasColumnName("logRelType")
                .HasConversion<Int32>()
                .IsRequired();

            objBuilder.Property(objLog => objLog.Text)
                .HasColumnName("logText")
                .HasColumnType("nvarchar(max)");

            objBuilder.HasIndex(objLog => objLog.Timestamp)
                .HasDatabaseName("IX_T_Log_logTimestamp");

            objBuilder.HasIndex(objLog => new
                {
                    objLog.LogType,
                    objLog.Timestamp
                })
                .HasDatabaseName("IX_T_Log_logType_logTimestamp");

            objBuilder.HasIndex(objLog => new
                {
                    objLog.LogArea,
                    objLog.Timestamp
                })
                .HasDatabaseName("IX_T_Log_logArea_logTimestamp");

            objBuilder.HasIndex(objLog => new
                {
                    objLog.MemberId,
                    objLog.Timestamp
                })
                .HasDatabaseName("IX_T_Log_logMemberId_logTimestamp");

            objBuilder.HasIndex(objLog => new
                {
                    objLog.RelType,
                    objLog.RelId,
                    objLog.Timestamp
                })
                .HasDatabaseName("IX_T_Log_logRelType_logRelId_logTimestamp");

            objBuilder.HasIndex(objLog => new
            {
                objLog.LogType,
                objLog.LogArea,
                objLog.Timestamp
            })
            .HasDatabaseName("IX_T_Log_logType_logArea_logTimestamp");
        }
    }
}