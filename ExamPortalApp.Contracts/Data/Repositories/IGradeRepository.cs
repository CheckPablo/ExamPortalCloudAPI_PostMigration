using ExamPortalApp.Contracts.Data.Entities;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface IGradeRepository : IRepositoryBase<Grade>
    {
        Task<IEnumerable<Grade>> GetAllByCenterIdAsync(int? id);
    }
}
