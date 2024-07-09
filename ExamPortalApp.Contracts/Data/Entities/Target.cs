﻿namespace ExamPortalApp.Contracts.Data.Entities;

public partial class Target
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? Removed { get; set; }
}
