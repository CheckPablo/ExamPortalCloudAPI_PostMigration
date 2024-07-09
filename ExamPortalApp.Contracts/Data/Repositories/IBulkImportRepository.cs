using ExamPortalApp.Contracts.Data.Entities;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface IBulkImportRepository : IRepositoryBase<StudentTest>
    {
        //Microsoft.AspNetCore.Mvc.JsonResult GetBatchID();
        Task <bool>ImportFile1(Stream stream, string batchGuid);
        Task <bool> ImportFile2(Stream stream, string batchGuid);
        Task <BulkImportPerson> GetBatchID();
        Task <bool> BulkImportExamPortalCloud(string batchGuidId);
    }
}
