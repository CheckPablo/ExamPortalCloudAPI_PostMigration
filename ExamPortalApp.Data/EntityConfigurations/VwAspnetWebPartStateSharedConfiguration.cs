using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetWebPartStateSharedConfiguration : IEntityTypeConfiguration<VwAspnetWebPartStateShared>
    {
        public void Configure(EntityTypeBuilder<VwAspnetWebPartStateShared> builder)
        {
            builder
                    .HasNoKey()
                    .ToView("vw_aspnet_WebPartState_Shared");

            builder.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
        }
    }
}
