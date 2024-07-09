namespace ExamPortalApp.Data.EntityConfigurations
{
    //internal class AStimulusTextConfiguration : IEntityTypeConfiguration<AStimulusText>
    //{
    //    public void Configure(EntityTypeBuilder<AStimulusText> builder)
    //    {
    //       builder.HasKey(e => e.StimulusTextId);

    //       builder.ToTable("aStimulusText");

    //       builder.HasIndex(e => e.StimulusId, "IX_aStimulusText").HasFillFactor(90);

    //       builder.HasIndex(e => e.StimulusTextId, "UQ__aStimulu__9B288AEFF1E8DA9C").IsUnique();

    //       builder.HasIndex(e => new { e.StimulusId, e.StimulusTextId }, "_dta_index_aStimulusText_18_1122103038__K2_K1_3").HasFillFactor(90);

    //       builder.Property(e => e.StimulusTextId).HasColumnName("StimulusTextID");
    //       builder.Property(e => e.ModifiedDate).HasColumnType("datetime");
    //       builder.Property(e => e.StimulusId).HasColumnName("StimulusID");
    //    }
    //}
}
