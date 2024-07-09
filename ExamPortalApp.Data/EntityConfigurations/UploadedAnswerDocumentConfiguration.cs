using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class UploadedAnswerDocumentConfiguration : IEntityTypeConfiguration<UploadedAnswerDocument>
    {
        public void Configure(EntityTypeBuilder<UploadedAnswerDocument> builder)
        {
        }
    }
}
