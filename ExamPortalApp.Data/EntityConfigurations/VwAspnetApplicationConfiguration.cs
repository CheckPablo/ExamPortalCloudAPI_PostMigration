using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetApplicationConfiguration : IEntityTypeConfiguration<VwAspnetApplication>
    {
        public void Configure(EntityTypeBuilder<VwAspnetApplication> builder)
        {
            builder
                    .HasNoKey()
                    .ToView("vw_aspnet_Applications");

            builder.Property(e => e.ApplicationName).HasMaxLength(256);
            builder.Property(e => e.Description).HasMaxLength(256);
            builder.Property(e => e.LoweredApplicationName).HasMaxLength(256);
        }
    }
}
