﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ExamPortalApp.Contracts.Data.Entities;

public partial class UploadedSourceDocument : EntityBase
{
    public int? TestId { get; set; }

    public string? FileName { get; set; }

    public byte[]? TestDocument { get; set; }

    public DateTime? DateModified { get; set; }

    public int? OldTestId { get; set; }
    [NotMapped]
    public string? SourceDocBase64 {get;set;} 

    public virtual Test? Test { get; set; }
}
