using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetProfileConfiguration : IEntityTypeConfiguration<VwAspnetProfile>
    {
        public void Configure(EntityTypeBuilder<VwAspnetProfile> builder)
        {
            builder
                    .HasNoKey()
                    .ToView("vw_aspnet_Profiles");
            builder.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
        }
    }
}
