using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class VwAspnetUsersInRoleConfiguration : IEntityTypeConfiguration<VwAspnetUsersInRole>
    {
        public void Configure(EntityTypeBuilder<VwAspnetUsersInRole> builder)
        {
            builder.HasNoKey()
                   .ToView("vw_aspnet_UsersInRoles");
        }
    }
}
