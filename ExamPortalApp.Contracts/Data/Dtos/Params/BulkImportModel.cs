namespace ExamPortalApp.Contracts;
using Microsoft.AspNetCore.Http;

public class BulkImportModel
{
  public BulkImportModel()
        {
        }
        public IFormFile PeopleExcel { get; set; }
        public IFormFile SubjectSectorExcel { get; set; }
}
