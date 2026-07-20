using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Users.Configurations
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> objEntity)
        {
            objEntity.ToTable("T_Role");

            objEntity.Property(x => x.Id)
                .HasColumnName("rolId");

            objEntity.Property(x => x.Name)
                .HasColumnName("rolName");

            objEntity.Property(x => x.NormalizedName)
                .HasColumnName("rolNameNorm");

            objEntity.Property(x => x.ConcurrencyStamp)
                .HasColumnName("rolConcurrencyStamp");
        }
    }
}