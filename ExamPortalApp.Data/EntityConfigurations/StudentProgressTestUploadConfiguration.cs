using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class StudentProgressTestUploadConfiguration : IEntityTypeConfiguration<StudentProgressTestUpload>
    {
        public void Configure(EntityTypeBuilder<StudentProgressTestUpload> builder)
        {
        }
    }
}
