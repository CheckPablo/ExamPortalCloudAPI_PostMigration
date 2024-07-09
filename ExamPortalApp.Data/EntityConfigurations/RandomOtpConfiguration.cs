using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class RandomOtpConfiguration : IEntityTypeConfiguration<RandomOtp>
    {
        public void Configure(EntityTypeBuilder<RandomOtp> builder)
        {
            builder.HasKey(e => new { e.CenterId, e.TestId, e.SectorId, e.SubjectId }).HasName("PK_RandomOTP");

            builder.ToTable("RandomOTPs");

            builder.HasIndex(e => e.Otp, "NonClusteredIndex-OTP");

            builder.Property(e => e.DateModified).HasColumnType("datetime");
            builder.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
            builder.Property(e => e.Otp).HasColumnName("OTP");
            builder.Property(e => e.OTPExpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("OTPExpiryDate");
        }
    }
}
