using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class IrregularityConfiguration : IEntityTypeConfiguration<Irregularity>
    {
        public void Configure(EntityTypeBuilder<Irregularity> builder)
        {
        }
    }
}
