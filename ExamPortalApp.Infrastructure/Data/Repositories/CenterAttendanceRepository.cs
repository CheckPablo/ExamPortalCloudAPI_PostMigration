using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class CenterAttendanceRepository : ICenterAttendanceRepository
    {
        private readonly IRepository _repository;


        public CenterAttendanceRepository(IRepository repository)
        {
            _repository = repository;
        }

        //public async Task<IEnumerable<CenterAttendance>> SearchAsync(int? centerId, int? sectorId, int? subjectId, int? testId)
        public async Task<IEnumerable<CenterAttendance>> SearchAsync(CenterAttendanceSearcher? searcher)
        {     //if (searcher?.SectorId == null || searcher?.SectorId == 0)
               //{ searcher?.EndExamDate = null}

            var parameters = new Dictionary<string, object>
            {
               
                { StoredProcedures.Params.SectorId, searcher?.SectorId},
                { StoredProcedures.Params.SubjectId, searcher?.SubjectId},
                { StoredProcedures.Params.TestID, 0},
                { StoredProcedures.Params.CenterID, searcher?.CenterId},
                { StoredProcedures.Params.ExamDate,searcher?.StartDate},                         
                { StoredProcedures.Params.EndExamDate, searcher?.EndExamDate }, 
           
            };
           // parameters.Add(searcher.EndExamDate,null)
             var result = await _repository.ExecuteStoredProcAsync<CenterAttendance>(StoredProcedures.GetCenterAttendance, parameters).ConfigureAwait(false);
            return result;
        }

   
    }
}