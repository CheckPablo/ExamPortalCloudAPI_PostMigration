using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class BufferATestQuestionConfiguration : IEntityTypeConfiguration<BufferATestQuestion>
    {
        public void Configure(EntityTypeBuilder<BufferATestQuestion> builder)
        {

            builder
                .HasNoKey()
                .ToTable("buffer_aTestQuestion");

            builder.Property(e => e.ModifiedDate).HasColumnType("datetime");
            builder.Property(e => e.QuestionId).HasColumnName("QuestionID");
            builder.Property(e => e.TestId).HasColumnName("TestID");
            builder.Property(e => e.TestQuestionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("TestQuestionID");
        }
    }
}
