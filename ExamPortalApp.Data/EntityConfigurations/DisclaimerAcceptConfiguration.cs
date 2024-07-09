using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class DisclaimerAcceptConfiguration : IEntityTypeConfiguration<DisclaimerAccept>
    {
        public void Configure(EntityTypeBuilder<DisclaimerAccept> builder)
        {
        }
    }
}
