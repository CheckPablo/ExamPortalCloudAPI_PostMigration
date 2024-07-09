using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetUserConfiguration : IEntityTypeConfiguration<VwAspnetUser>
    {
        public void Configure(EntityTypeBuilder<VwAspnetUser> builder)
        {
            builder
                    .HasNoKey()
                    .ToView("vw_aspnet_Users");

            builder.Property(e => e.LastActivityDate).HasColumnType("datetime");
            builder.Property(e => e.LoweredUserName).HasMaxLength(256);
            builder.Property(e => e.MobileAlias).HasMaxLength(16);
            builder.Property(e => e.UserName).HasMaxLength(256);
        }
    }
}
