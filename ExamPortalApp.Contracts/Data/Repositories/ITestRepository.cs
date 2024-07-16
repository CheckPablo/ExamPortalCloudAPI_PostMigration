﻿using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace ExamPortalApp.Contracts.Data.Repositories
{
    public interface ITestRepository : IRepositoryBase<Test>
    {
        Task<int> AddUpdateTestAsync(Test entity);
        Task<Test> UploadTestDocumentAsync(Test entity, IFormFile? file);
        Task<IEnumerable<StudentTestDTO>> GetStudentTestsBySectorCenterAndTestAsync(int? sectorId, int? centerId, int testId);
        Task<IEnumerable<Test>> SearchAsync(TestSearcher searcher);
        // Task<IEnumerable<Resulting>> SearchStudentAnswerAsync(StudentAnswerSearcher searcher);
        Task<IEnumerable<Resulting>> SearchStudentAnswerAsync(int gradeId, int subjectId, int testId, int regionId);
        Task<IEnumerable<StudentTestDTO>> GetStudentTestsLinksAsync(int? sectorId, int? centerId, int testId);
        Task<IEnumerable<RandomOtp>> GetOTP(TestOTPSearcher searcher);
        Task<bool> CreateNewOTPAsync(TestOTPSearcher otpGenerator);
        Task<bool> SendOTPToStudentsAsync(int id);
        Task<(Test, string)> GetTestWithFileAsync(int testId);
        Task<(Exam, string)> GetDBTestQuestionWithFileAsync(int testId, int studentId);
        Task<(Test, string)> GetTestQuestionWithFileAsync(int testId);
        Task<(Exam, string)> GetTestQuestionWithFileAsync(int testId, int studentId);
        Task<string> GetTestQuestionPaperTextFileAsync(int testId);
        Task<string> GetSourcePaperTextFileAsync(int testId);
         Task<string> GetTestQuestionPaperTextAsync(int testId);
        Task<(UserDocumentAnswer, string)> GetStudentFinalAnswerFileAsync(int testId, int studentId);
       // Task<(UserDocumentAnswer, string)> studentAnswersBulkDownload(int testId, int[] studentIds);
        Task<(UploadedAnswerDocument, string)> GetTestWithAnswerDocAsync(int id);
        Task<bool> LinkStudentsAsync(StudentTestLinker linker);
        //Task<bool> UploadSourceDocumentAsync(int testId, IFormFile file);
        Task<IEnumerable<UploadedSourceDocument>> UploadSourceDocumentAsync(int testId, IFormFile file);
        Task<IEnumerable<UploadedSourceDocument>> GetUploadedSourceDocumentsAsync(int testId);
        Task<IEnumerable<UploadedAnswerDocument>> GetUploadedAnswerDocumentAsync(int testId);
        Task<IEnumerable<UserDocumentAnswer>> GetUserAnswerDocumentAsync(int testId, int studentId);
        Task<IEnumerable<UserDocumentAnswer>> GetUserAnswerDocument(int testId, int studentId);
        Task<int> DeleteAnswerDocumentAsync(int id);
        Task<int> DeleteSourceDocumentAsync(int id);
        Task<string> GetFileAsync(int id, string type);
        //Task<bool> UploadAnswerDocumentAsync(int testId, IFormFile file);
        //Task<IEnumerable<UploadedAnswerDocument>> UploadAnswerDocumentAsync(int testId, IFormFile file);

        Task<bool> UploadAnswerDocumentAsync(int testId, IFormFile file);
        Task<string> GetWordFileAsync(int id);
        string ConvertWordDocToBase64Async(IFormFile file);
        //string ConvertOfflineString(string file);
        Task<byte[]> GetAnswerFileAsync(int testId);
        Task<byte[]> GetAnswerFileBytesAsync(int testId);
        Task<byte[]> GetUserAnswerFileAsync(int testId, int studentId);
        //Task<byte[]> GetUserAnswerFile(int testId, int studentId);
        Task<bool> CheckFileConvertedAsync(int id);
        byte[] ConvertAnswerDocumentAsync(IFormFile file);
        Task<Test> UploadTestWordDocumentAsync(Test entity, IFormFile file);
        Task<string> PreviewDocToUploadWord(Test entity, IFormFile file);
        Task<IEnumerable<Test>> GetOTPTestList(int? gradeId = null, int? subjectId = null);
        Task<byte[]> GetAudioFileAsync(int id);
        Task<IEnumerable<StudentTestList>> GetTestListAsync(int? studentId);
        Task<IEnumerable<StudentTestDTO>> SetTestStartDateTime(int? testId, int? studentId);
        Task<IEnumerable<RandomOtpDto>> ValidateTestOTP(int? testId, int? centerId, int? otp);
        Task <List<string> >StudentAnswersBulkDownload(int testId,int[] studentIds);
        Task<string> StudentAnswersBulkDownloadString(int testId, int[] studentIds);
        //Task studentAnswersBulkDownload(int[] documentIds);
        Task<Exam> LoadTestOnExamStart(int testId,int studentId);
        //Task <Test> UploadQuestionPaperDocAsync(Test entity,IFormFile? file);
       Task <IEnumerable<Test>> UploadQuestionPaperDocAsync(Test entity,IFormFile? file);
        //Task<IEnumerable<Student>>AddStudentAsync(Student student);
    }
}
