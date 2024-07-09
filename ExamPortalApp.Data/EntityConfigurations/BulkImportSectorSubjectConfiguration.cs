using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class BulkImportSectorSubjectConfiguration : IEntityTypeConfiguration<BulkImportSectorSubject>
    {
        public void Configure(EntityTypeBuilder<BulkImportSectorSubject> builder)
        {
        }
    }
}
