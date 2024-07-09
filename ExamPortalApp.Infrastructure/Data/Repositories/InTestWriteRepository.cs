﻿using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using ExamPortalApp.Infrastructure.Exceptions;
using ExamPortalApp.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class InTestWriteRepository : IInTestWriteRepository
    {
        private readonly IRepository _repository;
        //private readonly DecodedUser? _user;

        private readonly DecodedUser? _user ;
        private string textToSave;
        private IEnumerable<StudentTestAnswers> resultToReturn;
        private IEnumerable<ScannedImagesOTP> scannedImagesOTPResult;

        public InTestWriteRepository(IRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _user = accessor.GetLoggedInUser();
        }

        public async Task<StudentTest> AddAsync(StudentTest entity)
        {
            return await _repository.AddAsync(entity, true);
        }

        /*
        public Task ConvertWindowsTTS(WindowsSpeechModel winspeech)
        {
            throw new NotImplementedException();
        }*/

        public async Task<int> DeleteAsync(int id)
        {
            await _repository.DeleteAsync<StudentTest>(id);

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

        public async Task<StudentTest> UpdateAsync(StudentTest entity)
        {
            return await _repository.UpdateAsync(entity, true);
        }

        public async Task<StudentTestAnswers> UploadStudentAnswerDocumentAsync(int testId,int studentId, bool accomodation, 
            bool offline, bool fullsScreenClosed, bool KeyPress, bool leftEamArea, string timeRemaining, string answerText, 
            string fileName,IFormFile file)
        {
           
            var fileExtension = Path.GetExtension(file.FileName);

            if (!string.Equals(fileExtension, ".doc", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(fileExtension, ".docx", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Only Word Documents Supported");
            }
            //var base64 = 
            var fileBytes = file.ToByteArray();
            var base64BytConversion = fileBytes.ToBase64String();
            var convertedBytes = Convert.FromBase64String(base64BytConversion);

            using (MemoryStream stream = new(convertedBytes))
            {
                WordDocument document = new();
                document.Open(stream, FormatType.Docx);
                 foreach(WSection section in document.Sections) 
                { 
                    //Remove odd header 
                    section.HeadersFooters.OddHeader.ChildEntities.Clear(); 
                    //Remove even header 
                    section.HeadersFooters.EvenHeader.ChildEntities.Clear(); 
                    //Remove first page header 
                    section.HeadersFooters.FirstPageHeader.ChildEntities.Clear(); 
                    //Remove odd footer 
                    section.HeadersFooters.OddFooter.ChildEntities.Clear(); 
                    //Remove even footer 
                    section.HeadersFooters.EvenFooter.ChildEntities.Clear(); 
                    //Remove first page footer 
                    section.HeadersFooters.FirstPageFooter.ChildEntities.Clear(); 
                } 
                textToSave = document.GetText(); 
                 Console.WriteLine(textToSave);
                textToSave = textToSave.Replace("Created with a trial version of Syncfusion Word library or registered the wrong key in your application. Go to \"www.syncfusion.com/account/claim-license-key\" to obtain the valid key.", "");
            } 
          

            var answerDocument = await _repository.GetFirstOrDefaultAsync<UserDocumentAnswer>(x => x.TestId == testId && x.StudentId == studentId);
            //if (testId == 0)

              var answerDocumentData = new UserDocumentAnswer()
                {
                    //DateModified = DateTime.Now,
                   
                    FileName = file.FileName,
                    TestId = testId,
                    StudentId = studentId,
                    TestDocument = fileBytes,
                };

            if (answerDocument != null)
            {
                
                answerDocumentData.Id = answerDocument.Id;
                await _repository.UpdateAsync(answerDocumentData, true);
            }
            else
            {
                if (answerDocumentData != null)
                {
                    Console.WriteLine(answerDocumentData);
                }
                await _repository.AddAsync(answerDocumentData, true);
            }

            /*Please dont remove stored procedure option below***/
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.TestID, testId },
                { StoredProcedures.Params.StudentId, studentId },
                { StoredProcedures.Params.FileName, file.FileName },
                { StoredProcedures.Params.TestDocument, fileBytes },
                //{ StoredProcedures.Params.CenterID, _user.CenterId }
            };
            await _repository.ExecuteStoredProcAsync<UserCenter>(StoredProcedures.SaveStudentAnswer_TestUpload, parameters);

            await _repository.CompleteAsync();
            var trackingInfoToSave = new StudentTestAnswerModel
            {
                TestId = testId,
                StudentId = studentId,
                Accomodation = accomodation,
                Offline = offline,
                FullScreenClosed = fullsScreenClosed,
                KeyPress = KeyPress,
                LeftExamArea = leftEamArea,
                TimeRemaining = timeRemaining,
                AnswerText = textToSave,
                FileName = file.FileName
            };
            // answerText ?: string | null;
            var studentTestAnswers = await SaveAnswersInterval(trackingInfoToSave).ConfigureAwait(true); 
            return studentTestAnswers.FirstOrDefault();
        }

        /*public async Task<bool> UploadScannedImagesAsync(int testId, IFormFile file)
         {
             var test = await GetAsync(testId);
             var fileExtension = Path.GetExtension(file.FileName);

             if (!string.Equals(fileExtension, ".doc", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(fileExtension, ".docx", StringComparison.OrdinalIgnoreCase))
             {
                 throw new Exception("Only Word Documents Supported");
             }

             var fileBytes = file.ToByteArray();

             if (testId == 0)
             {
                 var answerDocument = new UploadedAnswerDocument
                 {
                     DateModified = DateTime.Now,
                     FileName = file.FileName,
                     TestId = test.Id,
                     TestDocument = fileBytes,
                 };

                 await _repository.AddAsync(answerDocument, true);
             }
             else
             {
                 var uploadedAnswerDocs = await _repository.GetFirstOrDefaultAsync<UploadedAnswerDocument>(x => x.TestId == testId);
                 //var uploadedAnswerDocs = await _repository.GetByIdAsync<UploadedAnswerDocument>(testId);
                 var answerDocument = new UploadedAnswerDocument()
                 {
                     TestId = testId,
                     FileName = file != null ? file.FileName : null,
                     TestDocument = fileBytes,
                     IsDeleted = false,
                 };
                 if (uploadedAnswerDocs != null)
                 {
                     uploadedAnswerDocs.FileName = answerDocument.FileName;
                     uploadedAnswerDocs.TestDocument = answerDocument.TestDocument;
                 }

                 if (uploadedAnswerDocs?.TestDocument == null && file != null)
                 {
                     await _repository.AddAsync(answerDocument, true);
                 }
                 if (uploadedAnswerDocs?.TestDocument != null && file != null)
                 {
                     await _repository.UpdateAsync(uploadedAnswerDocs, true);

                 }
             }

             return true;
         }*/

        public async Task<IEnumerable<StudentTestAnswers>> SaveAnswersInterval(StudentTestAnswerModel studentTestAnswersModel)
        {
             IEnumerable<StudentTestAnswers> result; 
            if (studentTestAnswersModel.AnswerText == null)
            {
                studentTestAnswersModel.AnswerText = "";
            }
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

            resultToReturn = await _repository.ExecuteStoredProcAsync<StudentTestAnswers>(StoredProcedures.StudentTestAnswersIntervalSave, parameters);
            return resultToReturn;
        }
        public async Task<int> UploadScannedImagetoDB(string?[] fileNames, string testId, string studentId)
        {
            if (fileNames?.Length == 0) throw new Exception("No Files uploaded");  
            Random random = new();
            var OTP = random.Next(10000, 99999);
            var expirydate = DateTime.Now.AddMinutes(10);
            
            foreach (var fileName in fileNames)
            {
                var parameters = new Dictionary<string, object>
                {
                    { StoredProcedures.Params.FileName, fileName },
                    { StoredProcedures.Params.OTP, OTP },
                    { StoredProcedures.Params.StudentId, studentId },
                    { StoredProcedures.Params.TestID, testId },
                    { StoredProcedures.Params.ExpiryDate, expirydate }
                };  
                _ = await _repository.ExecuteStoredProcAsync<ScannedImageResult>(StoredProcedures.UploadScannedImageDetails, parameters);
            }
            return OTP;
        }

        public async Task<IEnumerable<KeyPressTracking>> SaveIrregularKeyPress(InvalidKeyPressEntries invalidKeyPressEntries)
        {

            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, invalidKeyPressEntries.StudentId },
                { StoredProcedures.Params.TestID, invalidKeyPressEntries.TestId },
                { StoredProcedures.Params.Event, invalidKeyPressEntries.Event },
                { StoredProcedures.Params.Reason, invalidKeyPressEntries.Reason }
            };
       
            var result = await _repository.ExecuteStoredProcAsync<KeyPressTracking>(StoredProcedures.KeyPressTracking_ins, parameters);
            return result;
        }
        public async Task<Test> GetUserAnswerEntityAsync(int id)
        {
            var entity = await _repository.GetByIdAsync<Test>(id);

            if (entity == null) throw new EntityNotFoundException<Test>(id);

            return entity;
        }
        public async Task<IEnumerable<UserDocumentAnswer>> GetUserAnswerDocumentAsync(int studentId, int testId) // there are one or many testId in this tabe so USE STUDENTID
        {
            return await _repository.GetWhereAsync<UserDocumentAnswer>(x => x.StudentId == studentId && x.TestId == testId);
        }

        public async Task<IEnumerable<ScannedImageOTPResult>> VerifyImagesOTP(ScannedImagesOTP scannedImagesOTP)
        {
            //List<string> result = new List<string>();
             
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, scannedImagesOTP.StudentId },
                { StoredProcedures.Params.TestID, scannedImagesOTP.TestId },
                { StoredProcedures.Params.OTP, scannedImagesOTP.OTP }
            };
            var result = await _repository.ExecuteStoredProcAsync<ScannedImageOTPResult>(StoredProcedures.VerifyScannedImagesOTP, parameters);
            return result;
        }

        /*Task<IEnumerable<ScannedImageOTPResult>> IInTestWriteRepository.VerifyImagesOTP(ScannedImagesOTP scannedImagesOTP)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> VerifyImagesOTP(ScannedImageOTPResult scannedImagesOTP)
         {
             throw new NotImplementedException();
         }
         public Task<List<string>> VerifyImagesOTP(List<string> scannedImagesOTP)
 {
    throw new NotImplementedException();
 }*/

        /*Task<bool> IInTestWriteRepository.SaveIrregularKeyPress(InvalidKeyPressEntries invalidKeyPressEntries)
        {
            throw new NotImplementedException();
        }*/
    }
}
