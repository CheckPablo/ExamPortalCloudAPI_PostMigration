using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class UserScannedImageConfiguration : IEntityTypeConfiguration<UserScannedImage>
    {
        public void Configure(EntityTypeBuilder<UserScannedImage> builder)
        {
            builder.HasKey(e => e.Id).HasName("PK_tUserScannedImage");

            builder.HasIndex(e => new { e.StudentId, e.TestId, e.Otp }, "IX_Student_Test");

            builder.Property(e => e.Complete).HasDefaultValueSql("((0))");
            builder.Property(e => e.ExpiryDate).HasColumnType("datetime");
            builder.Property(e => e.Otp)
                .HasMaxLength(50)
                .HasColumnName("OTP");
        }
    }
}
