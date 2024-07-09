using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class TestSecurityLevelConfiguration : IEntityTypeConfiguration<TestSecurityLevel>
    {
        public void Configure(EntityTypeBuilder<TestSecurityLevel> builder)
        {
        }
    }
}
