using System.ComponentModel.DataAnnotations;

namespace ExamPortalApp.Contracts.Data.Dtos.Params
{
    public class StudentTestLoginModel
    {
        [Required]
        public string UniqueExamNo { get; set; } = string.Empty;
   
    }
}
