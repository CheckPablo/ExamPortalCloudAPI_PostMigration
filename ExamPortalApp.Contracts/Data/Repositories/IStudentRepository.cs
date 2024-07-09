using System.Runtime.CompilerServices;
using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface IStudentRepository : IRepositoryBase<Student>
    {
        bool CreateLoginCredentials(int[] studentIds);
        Task CreateLoginCredentialsAsync(Tuple<int, int[]> data);
        Task<List<string>> CreateLoginCredentialsAsync(int[] studentIds);
        Task<List<string>> SendLoginCredentialsAsync(int[] studentIds);
        Task<IEnumerable<InvigilatorStudentLinkResult>> GetInvigilatorStudentLinksAsync(int userId, int? gradeId = null, int? subjectId = null);
        Task<IEnumerable<Student>>AddStudentAsync(Student student);
        Task<IEnumerable<Student>> SearchAsync(StudentSearcher searcher);
        bool SendLoginCredentials(int[] studentIds);
        Task SendLoginCredentialsAsync(Dictionary<int, int[]> data);
        Task<bool> LinkStudentToSubjectsAsync(int studentId, int[] subjectIds);
        Task<bool> DelinkStudentToSubjectsAsync(int studentId, int[] subjectIds);
        Task PasswordMigration();
        Task <bool> SendRegistrationEmail(MailData student);
       
    }
}
