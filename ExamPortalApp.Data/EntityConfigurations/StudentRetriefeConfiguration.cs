using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class StudentRetriefeConfiguration : IEntityTypeConfiguration<StudentRetriefe>
    {
        public void Configure(EntityTypeBuilder<StudentRetriefe> builder)
        {
        }
    }
}
