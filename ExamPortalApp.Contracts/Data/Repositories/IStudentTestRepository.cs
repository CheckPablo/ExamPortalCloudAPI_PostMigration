﻿using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface IStudentTestRepository : IRepositoryBase<StudentTest>
    {
        Task<bool> AcceptDisclaimer(int testId, int studentId, bool isDisclaimerAccepted);
        Task<IEnumerable<StudentTestAnswers>> SaveAnswersInterval(StudentTestAnswerModel studentTestAnswers);

        Task<IEnumerable<StudentTestAnswers>> GetStudentTestDetails(int testId, int studentId);

        Task<IEnumerable<StudentTestAnswers>> FinishTest(StudentTestAnswers studentTestAnswers);
        Task<IEnumerable<UserDocumentAnswer>> GetUserAnswerDocumentAsync(int testId, int studentId);

    }
}
