﻿using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamPortalApp.Data.EntityConfigurations
{
    internal class BcktQuestionConfiguration : IEntityTypeConfiguration<BcktQuestion>
    {
        public void Configure(EntityTypeBuilder<BcktQuestion> builder)
        {
            builder.HasNoKey()
                    .ToTable("bcktQuestion");
            builder.Property(e => e.ModifiedDate).HasColumnType("datetime");
            builder.Property(e => e.NoteId)
                .IsUnicode(false)
                .HasColumnName("NoteID");
            builder.Property(e => e.QuestionCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            builder.Property(e => e.QuestionId).HasColumnName("QuestionID");
            builder.Property(e => e.QuestionInstruction).IsUnicode(false);
            builder.Property(e => e.QuestionStem).IsUnicode(false);
            builder.Property(e => e.QuestionTypeId).HasColumnName("QuestionTypeID");
            builder.Property(e => e.StimulusId).HasColumnName("StimulusID");
        }
    }
}
