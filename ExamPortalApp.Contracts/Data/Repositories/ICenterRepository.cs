using ExamPortalApp.Contracts.Data.Entities;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface ICenterRepository : IRepositoryBase<Center>
    {
        Task<IEnumerable<Center>> GetCenterByUser();
        Task<IEnumerable<Center>> SearchCenterSummaryAsync();
        Task<IEnumerable<Center>> GetCenterByUserId(int centerId);
         //Task<IEnumerable<Center>> GetCenterByUserId(string username);
    }
    //Task<IEnumerable<StudentTestDTO>> GetStudentTestsBySectorCenterAndTestAsync(int? sectorId, int? centerId, int testId);
}
