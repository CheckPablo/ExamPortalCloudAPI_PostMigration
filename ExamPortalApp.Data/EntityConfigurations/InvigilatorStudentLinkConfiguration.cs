using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class InvigilatorStudentLinkConfiguration : IEntityTypeConfiguration<InvigilatorStudentLink>
    {
        public void Configure(EntityTypeBuilder<InvigilatorStudentLink> builder)
        {
        }
    }
}
