using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class UserDocumentAnswerConfiguration : IEntityTypeConfiguration<UserDocumentAnswer>
    {
        public void Configure(EntityTypeBuilder<UserDocumentAnswer> builder)
        {
        }
    }
}
