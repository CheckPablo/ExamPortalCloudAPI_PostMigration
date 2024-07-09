using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class UploadedTestConfiguration : IEntityTypeConfiguration<UploadedTest>
    {
        public void Configure(EntityTypeBuilder<UploadedTest> builder)
        {
        }
    }
}
