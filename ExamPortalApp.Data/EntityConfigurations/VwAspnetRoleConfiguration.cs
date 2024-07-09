using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetRoleConfiguration : IEntityTypeConfiguration<VwAspnetRole>
    {
        public void Configure(EntityTypeBuilder<VwAspnetRole> builder)
        {
            builder
                    .HasNoKey()
                    .ToView("vw_aspnet_Roles");

            builder.Property(e => e.Description).HasMaxLength(256);
            builder.Property(e => e.LoweredRoleName).HasMaxLength(256);
            builder.Property(e => e.RoleName).HasMaxLength(256);
        }
    }
}
