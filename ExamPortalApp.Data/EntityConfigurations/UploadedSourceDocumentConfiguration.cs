using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class UploadedSourceDocumentConfiguration : IEntityTypeConfiguration<UploadedSourceDocument>
    {
        public void Configure(EntityTypeBuilder<UploadedSourceDocument> builder)
        {

        }
    }
}
