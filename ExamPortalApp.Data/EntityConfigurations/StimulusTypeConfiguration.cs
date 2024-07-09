using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class StimulusTypeConfiguration : IEntityTypeConfiguration<StimulusType>
    {
        public void Configure(EntityTypeBuilder<StimulusType> builder)
        {
        }
    }
}
