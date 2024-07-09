using System.ComponentModel.DataAnnotations.Schema;

namespace ExamPortalApp.Contracts.Data.Entities;

public partial class UploadedTest : EntityBase
{
    public string? FileName { get; set; }

    public byte[]? TestDocument { get; set; }

    [NotMapped]
    public string? TestDocBase64 { get; set; }
}
