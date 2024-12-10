 using System.ComponentModel.DataAnnotations.Schema;

namespace ExamPortalApp.Contracts.Data.Entities;

public partial class UploadedTestCacheLog : EntityBase
{
    public int testId { get; set; }
    public string? FileName { get; set; }
    public string? TestDocCacheValue { get; set; }
    public DateTime? DateModified { get; set; }
    
} 
