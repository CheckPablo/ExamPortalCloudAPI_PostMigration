﻿namespace ExamPortalApp.Contracts.Data.Entities;

public partial class Note
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? Reference { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? Removed { get; set; }
}
