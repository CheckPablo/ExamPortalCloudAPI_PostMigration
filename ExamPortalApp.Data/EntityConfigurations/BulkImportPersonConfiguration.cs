﻿using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class BulkImportPersonConfiguration : IEntityTypeConfiguration<BulkImportPerson>
    {
        public void Configure(EntityTypeBuilder<BulkImportPerson> builder)
        {
        }
    }
}