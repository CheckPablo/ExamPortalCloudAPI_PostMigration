using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class StimulusMediumConfiguration : IEntityTypeConfiguration<StimulusMedium>
    {
        public void Configure(EntityTypeBuilder<StimulusMedium> builder)
        {
        }
    }
}
