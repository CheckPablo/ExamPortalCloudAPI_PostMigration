using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class StudentTestLogConfiguration : IEntityTypeConfiguration<StudentTestLog>
    {
        public void Configure(EntityTypeBuilder<StudentTestLog> builder)
        {
        }
    }
}
