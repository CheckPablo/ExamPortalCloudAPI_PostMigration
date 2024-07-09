using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using ExamPortalApp.Infrastructure.Exceptions;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class StudentTestRepository(IRepository repository) : IStudentTestRepository
    {
        private readonly IRepository _repository = repository;

        public async Task<bool> AcceptDisclaimer(int testId, int studentId, bool isDisclaimerAccepted)
        {
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.approve, testId },
                { StoredProcedures.Params.active, studentId },
                { StoredProcedures.Params.CenterID, isDisclaimerAccepted }
            };
            var result = _repository.ExecuteStoredProcAsync<StudentTest>(StoredProcedures.AcceptDisclaimerInsert, parameters);
            return result is not null;
        }

        public async Task<StudentTest> AddAsync(StudentTest entity)
        {
            return await _repository.AddAsync(entity, true);
        }

       /*  public async Task<int> DeleteAsync(int id)
        {
            await _repository.DeleteAsync<StudentTest>(id);

            return await _repository.CompleteAsync();
        } */

         public async Task<int> DeleteAsync(int id)
        {
           // await _repository.DeleteAsync<StudentTest>(id);
               var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, id},
            
            };
             //var result = await _repository.ExecuteStoredProcAsync<Student>(StoredProcedures.StudentMaintenanceDelete, parameters);
            //return result;
           await _repository.ExecuteStoredProcAsync<Student>(StoredProcedures.StudentMaintenanceDelete, parameters);
            //return result;
           return await _repository.CompleteAsync();
        }

        public async Task<IEnumerable<StudentTest>> GetAllAsync()
        {
            return await _repository.GetAllAsync<StudentTest>();   
        }

        public async Task<StudentTest> GetAsync(int id)
        {
            var entity = await _repository.GetByIdAsync<StudentTest>(id);

            if (entity == null) throw new EntityNotFoundException<StudentTest>(id);

            return entity;
        }

        public async Task<IEnumerable<UserDocumentAnswer>> GetUserAnswerDocumentAsync(int studentId, int testId) // there are one or many testId in this tabe so USE STUDENTID
        {
            return await _repository.GetWhereAsync<UserDocumentAnswer>(x => x.StudentId == studentId && x.TestId == testId);
        }

        public async Task<IEnumerable<StudentTestAnswers>> SaveAnswersInterval(StudentTestAnswerModel studentTestAnswersModel)
       {
            studentTestAnswersModel.AnswerText ??= "";
            
            if (studentTestAnswersModel.AnswerText.Length == 0)
            {
                studentTestAnswersModel.AnswerText = "";
            }
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, studentTestAnswersModel.StudentId },
                { StoredProcedures.Params.TestID, studentTestAnswersModel.TestId },
                { StoredProcedures.Params.TimeRemaining, studentTestAnswersModel.TimeRemaining },
                { StoredProcedures.Params.KeyPress, studentTestAnswersModel.KeyPress },
                { StoredProcedures.Params.LeftExamArea, studentTestAnswersModel.LeftExamArea },
                { StoredProcedures.Params.Offline, studentTestAnswersModel.Offline },
                { StoredProcedures.Params.FullScreenClosed, studentTestAnswersModel.FullScreenClosed },
                { StoredProcedures.Params.FileName, studentTestAnswersModel.FileName },
                { StoredProcedures.Params.AnswerText, studentTestAnswersModel.AnswerText },
                { StoredProcedures.Params.Accomodation, studentTestAnswersModel.Accomodation }
            };

            var result = await _repository.ExecuteStoredProcAsync<StudentTestAnswers>(StoredProcedures.StudentTestAnswersIntervalSave, parameters);
            return result;
        }


        public async Task<StudentTest> UpdateAsync(StudentTest entity)
        {
            return await _repository.UpdateAsync(entity, true);
        }

        public Task<IEnumerable<StudentTestAnswers>> GetStudentTestDetails(int testId, int studentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.TestID, testId },
                { StoredProcedures.Params.StudentId, studentId }
            };


            var result = _repository.ExecuteStoredProcAsync<StudentTestAnswers>(StoredProcedures.GetStudentTestDetails, parameters);
            return result;
        }


        public async Task<IEnumerable<StudentTestAnswers>> FinishTest(StudentTestAnswers studentTestAnswersModel)
        {
            
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.TestID, studentTestAnswersModel.TestID },
                { StoredProcedures.Params.StudentId, studentTestAnswersModel.StudentId }
            };

            var result = await _repository.ExecuteStoredProcAsync<StudentTestAnswers>(StoredProcedures.CompleteTest, parameters);
            return result;
        }
    }
}
