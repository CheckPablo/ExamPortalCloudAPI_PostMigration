﻿using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class CenterTypeConfiguration : IEntityTypeConfiguration<CenterType>
    {
        public void Configure(EntityTypeBuilder<CenterType> builder)
        {
        }
    }
}
