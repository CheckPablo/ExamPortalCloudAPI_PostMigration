using ExamPortalApp.Contracts.Data.Dtos;
using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Contracts.Data.Dtos.Params;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using ExamPortalApp.Infrastructure.Exceptions;
using ExamPortalApp.Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class StudentRepository(IRepository repository, IGradeRepository gradeRepository, IHttpContextAccessor accessor, IBackgroundQueue<Dictionary<int, int[]>> resendQueue,
        IBackgroundQueue<Tuple<int, int[]>> createQueue, IEmailService emailService, IOptions<ExamPortalSettings> options) : IStudentRepository
    {
        private readonly IBackgroundQueue<Dictionary<int, int[]>> _resendQueue = resendQueue;
        private readonly IBackgroundQueue<Tuple<int, int[]>> _createQueue = createQueue;
        private readonly ExamPortalSettings _examPortalSettings = options.Value;
        private readonly IEmailService _emailService = emailService;
        private readonly IRepository _repository = repository;
        private readonly IGradeRepository _gradeRepository = gradeRepository;
        private readonly DecodedUser? _user = accessor.GetLoggedInUser();
        private IEnumerable<StudentSubject> linkResult;

        public async Task<IEnumerable<Student>> AddStudentAsync(Student entity)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var center = await _repository.GetByIdAsync<Center>(_user.CenterId, x => x.Students);

            if (center is null) throw new Exception(ErrorMessages.Auth.Unauthorised);
            if (center.MaximumLicense <= center.Students.Count()) throw new InvalidOperationException("Maximum number of students reached.");
            var studentExists = await _repository.AnyAsync<Student>(x => x.StudentNo == entity.StudentNo);
            if (studentExists)
            {
                //throw new Exception(ErrorMessages.StudentEntryChecks.StudentExists);
                throw new InvalidStudentEntryException(); 
            }
            var password = GeneralHelpers.RandomPassword(8);

            entity.CenterId = center.Id;
            entity.ModifiedBy = _user.Id;
            entity.CertLangId = 1; 
            entity.EncrytedPassword = PasswordHelper.Encrypt(password, _examPortalSettings.EncryptionKey);

            //var student = await _repository.AddAsync(entity, true);
            
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, entity.Id },
                { StoredProcedures.Params.Name, entity.Name },
                { StoredProcedures.Params.Surname, entity.Surname },
                { StoredProcedures.Params.CenterID, center.Id },
                { StoredProcedures.Params.IDNumber, entity.IdNumber },
              
                { StoredProcedures.Params.CertLangID, entity.CertLangId },
                { StoredProcedures.Params.ModifiedBy, _user.Id },
                { StoredProcedures.Params.SectorId, entity.GradeId },
                { StoredProcedures.Params.Email, entity.EmailAddress },
                { StoredProcedures.Params.ContactNo, entity.ContactNo },
                { StoredProcedures.Params.RegionId, entity.RegionId },
                { StoredProcedures.Params.EncryptedPassword, entity.EncrytedPassword },
              
                { StoredProcedures.Params.StudentNo, entity.StudentNo },
                { StoredProcedures.Params.ExternalEmail, entity.ExternalEmail },
                { StoredProcedures.Params.EligibleForExternalLogin, entity.EligibleForExternalLogin },
              
            };
            try{
                  var student = await _repository.ExecuteStoredProcAsync<Student>(StoredProcedures.StudentMaintenance_InsertUpdate_Revised, parameters);
                  entity.ExamNo = await CreateNewExamNumber(center.Prefix, student.FirstOrDefault().Id);
                   //SendRegistrationEmail(student.First(), password);
                   student.First().PlainPassword = password; 
                   await _repository.UpdateAsync(entity, true);
                  return student.ToList();
           /*   var student = _repository.ExecuteStoredProcedure<Student>(StoredProcedures.StudentMaintenance_InsertUpdate_Revised, parameters);
              return (Student)student; */
            }catch(Exception ex){
                throw new Exception("", ex);    
            }
        /* 
            var examNo = CreateExamNumber(center.Prefix, student.Id);

            student.ExamNo = examNo;
            entity.ExamNo = examNo;
            SendRegistrationEmail(entity, password);
            //await _repository.UpdateAsync(student, true);
            //return student;
            return (Student)student; */
        }
/*  public Task<Student> AddAsync(Student entity)
 {
        {
            throw new NotImplementedException();
        }
    } */
        public static string CreateExamNumber(string? prefix, int studentId)
        {
            var random = new Random();
            string randomPart = (random.NextDouble().ToString("F11").Substring(2, 2) + random.NextDouble().ToString("F11").Substring(3, 10)).Substring(0, 2);
            string uniqueExamNo = $"{prefix}{randomPart}{(studentId.ToString("D9"))}";

            return uniqueExamNo;
        }

          public async Task<string> CreateNewExamNumber(string? prefix, int studentId)
        {
            var random = new Random();
            string randomPart = (random.NextDouble().ToString("F11").Substring(2, 2) + random.NextDouble().ToString("F11").Substring(3, 10)).Substring(0, 2);
            string uniqueExamNo = $"{prefix}{randomPart}{(studentId.ToString("D9"))}";

            return uniqueExamNo;
        }

        public bool CreateLoginCredentials(int[] studentIds)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            Tuple<int, int[]> data = Tuple.Create(_user.Id, studentIds);

            _createQueue.Enqueue(data);

            return true;
        }
        public async Task CreateLoginCredentialsAsync(Tuple<int, int[]> data)
        {
            var (userId, studentIds) = data;
            var user = await _repository.GetByIdAsync<User>(userId);

            if (user is null) return;

            var center = await _repository.GetByIdAsync<Center>(user.CenterId);

            if (center is null) return;

            await ProcessStudentIdsAsync(userId, studentIds, center);
        }
        public async Task<List<string>> CreateLoginCredentialsAsync(int[] studentIds)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var center = await _repository.GetByIdAsync<Center>(_user.CenterId) ?? throw new Exception(ErrorMessages.Auth.CenterMissing);
            var students = await SendStudentLoginCredentialsAsync(_user.Id, studentIds, center,true);
            return students;
        }

        public async Task<List<string>> SendLoginCredentialsAsync(int[] studentIds)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var center = await _repository.GetByIdAsync<Center>(_user.CenterId) ?? throw new Exception(ErrorMessages.Auth.CenterMissing);
            var students = await SendStudentLoginCredentialsAsync(_user.Id, studentIds, center);
            return students;
        }

      /*   public async Task<int> DeleteAsync(int id)
        {
            await _repository.DeleteAsync<Student>(id);
            var studentTestsLink = await _repository.GetWhereAsync<StudentTest>(x => x.StudentId == id);
            var studentSubjectsLink = await _repository.GetWhereAsync<StudentSubject>(x => x.StudentId == id);
            //var sourceDocs = await _repository.GetWhereAsync<TestQuestion>(x => x.TestId == id);
            if (studentTestsLink != null)
            {
                foreach (var stl in studentTestsLink)
                    await _repository.DeleteAsync<StudentTest>(stl.StudentId ?? 0);
            }

            if (studentSubjectsLink != null)
            {
                foreach (var ssl in studentSubjectsLink)
                    await _repository.DeleteAsync<StudentTest>(ssl.StudentId ?? 0);
            }

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

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            //var students = await _repository.GetAllAsync<Student>();

            //foreach (var student in students)
            //{
            //    if (student.EncrytedPassword is null)
            //    {
            //        var password = PasswordHelper.GeneratePassword();

            //        student.EncrytedPassword = PasswordHelper.Encrypt(password, _examPortalSettings.EncryptionKey);

            //        await _repository.UpdateAsync(student);
            //    }
            //}

            //await _repository.CompleteAsync();

            //return students;
            //return await _repository.GetAllAsync<Student>();
            if (_user == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            var students = await _repository.GetWhereAsync<Student>(x => x.CenterId == _user.CenterId);
            //return students.OrderBy(x => x.Id);
            return students.OrderBy(x => x.Id);
        }

        public async Task<Student> GetAsync(int id)
        {
            var student = await _repository.GetByIdAsync<Student>(id, x => x.StudentSubjects);

            if (student is null) throw new EntityNotFoundException<Student>(id);

            return student;
        }

        public async Task<IEnumerable<InvigilatorStudentLinkResult>> GetInvigilatorStudentLinksAsync(int userId, int? gradeId = null, int? subjectId = null)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var students = await _repository.GetWhereAsync<Student>(x => x.CenterId == _user.CenterId,
                x => x.StudentSubjects, x => x.InvigilatorStudentLinks);

            if (gradeId is not null) students = students.Where(x => x.GradeId == gradeId).ToList();
            if (subjectId is not null) students = students.Where(x => x.StudentSubjects.Any(y => y.SubjectId == subjectId)).ToList();

            var result = students.Select(x => new InvigilatorStudentLinkResult
            {
                ExamNo = x.ExamNo,
                IdNumber = x.IdNumber,
                Linked = x.InvigilatorStudentLinks.Any(x => x.InvigilatorId == userId),
                StudentId = x.Id,
                Name = x.Name,
                Surname = x.Surname,
            });

            return result.OrderBy(x => x.Name).ThenBy(x => x.Surname);
        }

        private static IEnumerable<Student> GetOrdered(IEnumerable<Student> students) => students.OrderBy(x => x.Name).ThenBy(x => x.Surname);

        /* public async Task LinkStudentToSubjectsAsync(int studentId, int[] subjectIds)
         {
             var currenctLinks = await _repository.GetWhereAsync<StudentSubject>(x => x.StudentId == studentId);

             foreach (var subjectId in subjectIds)
             {
                 var studentSubject = currenctLinks.FirstOrDefault(x => x.SubjectId == subjectId && x.StudentId == studentId);

                 if (studentSubject != null)
                 {
                     currenctLinks = currenctLinks.Where(x => x.Id != studentSubject.Id);
                 }
                 else
                 {
                     studentSubject = new StudentSubject
                     {
                         SubjectId = subjectId,
                         StudentId = studentId,
                     };

                     await _repository.AddAsync(studentSubject);
                     await _repository.CompleteAsync();
                 }
             }

             foreach (var studentSubject in currenctLinks)
             {
                 await _repository.DeleteAsync<StudentSubject>(studentSubject.Id);
             }

             await _repository.CompleteAsync();
         }*/

        public async Task<bool> LinkStudentToSubjectsAsync(int studentId, int[] subjectIds)
        {
            _ = await DeleteSubjectStudentLinks(studentId);
            var parameters = new Dictionary<string, object>();
            foreach (var subjectId in subjectIds)
            {
                parameters.Clear();
                parameters.Add(StoredProcedures.Params.StudentId, studentId);
                parameters.Add(StoredProcedures.Params.SubjectId, subjectId);

                linkResult = await _repository.ExecuteStoredProcAsync<StudentSubject>(StoredProcedures.LinkStudentSubjects, parameters);
            }
            return linkResult is not null;
        }

        public async Task<bool> DelinkStudentToSubjectsAsync(int studentId, int[] subjectIds)
        {
            //DeleteSubjectStudentLinks(studentId);
           var parameters = new Dictionary<string, object>
           {
               { StoredProcedures.Params.StudentId, studentId }
           };

            linkResult = await _repository.ExecuteStoredProcAsync<StudentSubject>(StoredProcedures.DeleteStudentSubjects, parameters);
      
            return linkResult is not null;
        }

        private async Task<bool> DeleteSubjectStudentLinks(int studentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.StudentId, studentId }
            };
            var result = _repository.ExecuteStoredProcedureAsync<StudentSubject>(StoredProcedures.DeleteStudentSubjects, parameters);

            return result is not null;
        }

        private async Task ProcessStudentIdsAsync(int userId, int[] studentIds, Center? center = null)
        {
            List<string> students = [];
            var user = await _repository.GetByIdAsync<User>(userId) ?? throw new Exception(ErrorMessages.Auth.Unauthorised);
            foreach (var studentId in studentIds)
            {
                var student = await _repository.GetByIdAsync<Student>(studentId);
                var password = string.Empty;

                if (student is null) continue;
                if (student.EncrytedPassword is not null)
                {
                    password = PasswordHelper.Decrypt(student.EncrytedPassword, _examPortalSettings.EncryptionKey);
                }

                if (center is not null)
                {
                    student.ExamNo = CreateExamNumber(center.Prefix, studentId);
                }

                await _repository.UpdateAsync(student);

                SendLoginCredentials(student, password);

                students.Add($"{student.Name} {student.Surname}");
            }
        }

        private async Task<List<string>> SendStudentLoginCredentialsAsync(int userId, int[] studentIds, Center? center = null,bool createExamNo = false )
        {
            List<string> students = [];
            foreach (var studentId in studentIds)
            {
                var student = await _repository.GetByIdAsync<Student>(studentId);
                var password = string.Empty;

                if (student is null) continue;
                
                // if password exist take from DB
                if (student.EncrytedPassword is not null)
                {
                    password = PasswordHelper.Decrypt(student.EncrytedPassword, _examPortalSettings.EncryptionKey);
                }
                
                //else create password
                if(student.EncrytedPassword is null){
                 password = GeneralHelpers.RandomPassword(8);
           
                 //entity.ModifiedBy = _user.Id;
                 //entity.ExamNo = password;
                 student.EncrytedPassword = PasswordHelper.Encrypt(password, _examPortalSettings.EncryptionKey);
                }

                if (center is not null && createExamNo)
                {
                    student.ExamNo = CreateExamNumber(center.Prefix, studentId);
                }

                if(student.ExamNo == "ExamNumbe" || student.ExamNo is null){
                  student.ExamNo = CreateExamNumber(center?.Prefix, studentId);
                }
                
                await _repository.UpdateAsync(student);

                SendLoginCredentials(student, password);

                students.Add($"{student.Name} {student.Surname}");
            }
            return students;
        }
        public async Task<IEnumerable<Student>> SearchAsync(StudentSearcher? searcher)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var query = _repository.GetQueryable<Student>()
                .Include(s => s.Grade)
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .Where(x => x.CenterId == _user.CenterId);

            if (searcher?.GradeId is not null) query = query.Where(x => x.GradeId == searcher.GradeId);
            if (searcher?.SubjectId is not null) query = query.Where(s => s.StudentSubjects.Any(ss => ss.SubjectId == searcher.SubjectId));
            if (!string.IsNullOrWhiteSpace(searcher?.Name)) query = query.Where(s => s.Name != null && s.Name.Contains(searcher.Name.ToLower().Trim()));

            var data = await query.ToListAsync();

            var result = data.Select(x => new Student
            {
                Id = x.Id,
                ExamNo = x.ExamNo,
                IdNumber = x.IdNumber,
                EmailAddress = x.EmailAddress,
                Name = x.Name,
                Surname = x.Surname,
                Grade = new Grade
                {
                    Id = x.Grade.Id,
                    Code = x.Grade?.Code,
                },
                PlainPassword = PasswordHelper.Decrypt(x.EncrytedPassword, _examPortalSettings.EncryptionKey),
                //PlainPassword = PasswordHelper.DecryptSHA(x.PasswordEncrypted, _examPortalSettings.EncryptionKey),

                SentConfirmation = !String.IsNullOrEmpty(x.EncrytedPassword),

                //SentConfirmation = (x.SentConfirmation.HasValue) ? x.SentConfirmation.Value : (!String.IsNullOrEmpty(x.ExamNo) | !String.IsNullOrEmpty(x.PlainPassword) | (x.PlainPassword.Length < 1)) ? true : false,
                //SentConfirmation = // if (String.IsNullOrEmpty(PlainPassword))
                // if (String.IsNullOrEmpty(PlainPassword))
            }); ;
            Console.WriteLine(result);
            return GetOrdered(result);
        }


        public bool SendLoginCredentials(int[] studentIds)
        {
            if (_user is null) throw new Exception(ErrorMessages.Auth.Unauthorised);

            var data = new Dictionary<int, int[]>
            {
                { _user.Id, studentIds }
            };

            _resendQueue.Enqueue(data);

            return true;
        }

        private static void SendLoginCredentials(Student student, string password)
        {
            if (student.EmailAddress is null) return;

            //var body = $"Hi {student.Name} 🙋🏽‍, \n\nWe are sending you your Exam Portal Cloud credentails below \n\n"
            //    + " Username: " + student.ExamNo + "\n"
            //    + " Password: " + password + "\n\n"
            //    + " Use the following link to login : www.examportalcloud.co.za ";

            //var email = new EmailDto
            //{
            //    EmailAddesses = new List<string> { student.EmailAddress },
            //    MessageBody = body,
            //    Subject = "Exam Portal Cloud Credentials"
            //};

            //_emailService.SendOrQueue(email, true);
            MailMessage mail = new()
            {
                //var mailAd = users.Last(x => x.UserEmailAddress.Length > 0);
                //From = new MailAddress("qiscmapp@gmail.com")
                From = new MailAddress("Support@v-soft.co.za"),
            };
            //mail.From = new MailAddress("qiscmapp@gmail.com");

            mail.To.Add("Tinashe@v-soft.co.za");
            mail.To.Add(student.EmailAddress);
            mail.CC.Add("support@v-soft.co.za");
            mail.Bcc.Add("syntaxdon@gmail.com");
            mail.Subject = " Login Details for Exam Portal Cloud";

            mail.Body = " Dear" + "".PadRight(1) + student.Name + "" + " ,\n\n"
            + " Your credentials have been sent. \n \n"
            + " Your New login details: \n \n"
            + " Exam Number: " + student.ExamNo + "\n"
            + " Password: " + password + "\n \n"
            + " Use the following link to login : https://examportalcloud.co.za/ . \n \n "
            + " IMPORTANT: Please ensure you have Safe Exam Browser installed your on your Windows / Mac computer/laptop. You may download it from here: https://sourceforge.net/projects/seb/files/seb/SEB_2.4.1/SafeExamBrowserInstaller.exe/download \n \n"

            + " If you experience any problems during login or during your examination, please contact your exam invigilator immediately. \n \n"
            + " Kind Regards, \n"
            + " The Exam Portal Cloud team";
            SmtpClient smtpServer = new()
            {
                //smtpServer.UseDefaultCredentials = false;;
               /*  Host = "smtp.gmail.com",
                Port = 587, */
                Host = "mail.smtp2go.com",
                Port = 2525,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Credentials = new NetworkCredential("qiscmapp@gmail.com", "gkrikvoauqlshyzg"),
                Credentials = new NetworkCredential("Support@v-soft.co.za", "*VSoft*2019"),
                Timeout = 20000
            };
            smtpServer.Send(mail);
        }

        public async Task SendLoginCredentialsAsync(Dictionary<int, int[]> data)
        {
            foreach (var item in data)
            {
                await ProcessStudentIdsAsync(item.Key, item.Value);
            }
        }

        private static void SendLoginCredentialsConfirmation(User user, string rows)
        {
            //if (user.UserEmailAddress is null) return;

            //var body = $"Hi {user.Name} 🙋🏽‍, \n\nWe have sent the following credentials on your behalf: \n\n"
            //    + "<table class='width:100%'>"
            //    + "<th><td>Name</td><td>Email</td><td>Exam No.</td><td>New Password</td></th>"
            //    + $"<tbody>{rows}</tbody>"
            //    + "</table>";

            //var email = new EmailDto
            //{
            //    EmailAddesses = new List<string> { user.UserEmailAddress },
            //    MessageBody = body,
            //    Subject = "Exam Portal Cloud Credentials"
            //};

            //_emailService.SendOrQueue(email, true);
        }

        private bool SendRegistrationEmail(Student student, string password)
        {
            //if (student.EmailAddress is null) return;

            //var body = "Dear User: \n \n"
            //    + " Welcome to Exam Portal Cloud \n \n"
            //    + " Your Login Registration details: \n \n"
            //    + " Exam Number: " + student.ExamNo + "\n"
            //    + " Password: " + password + "\n \n"
            //    + " Use the following link to login : https://examportalcloud.co.za/ and select Student Login. \n \n "
            //    + " IMPORTANT: Please ensure you have Safe Exam Browser installed on your Windows / Mac computer/laptop. You may download it from here: https://sourceforge.net/projects/seb/files/seb/SEB_2.4.1/SafeExamBrowserInstaller.exe/download  If you do not have a Windows or a Mac book computer / laptop, you do not need to install Safe Exam Browser. \n \n"
            //    + " If you experience any problems during login or during your examination, please contact your exam invigilator immediately. \n \n"
            //    + " Kind Regards, \n"
            //    + " The Exam Portal Cloud team";

            //var emailDto = new EmailDto
            //{
            //    EmailAddesses = new List<string> { student.EmailAddress },
            //    MessageBody = body,
            //    Subject = "Login Details for Exam Portal Cloud",

            //};

            //_emailService.SendOrQueue(emailDto);
            MailMessage mail = new();
            //sending when emailing a studenton add student
            mail.From = new MailAddress("qiscmapp@gmail.com");
            //mail.From = new MailAddress("Support@v-soft.co.za");
            mail.To.Add("Tinashe@v-soft.co.za");
            mail.To.Add(student.EmailAddress);
            //mail.Bcc.Add("support@v-soft.co.za");
            mail.Bcc.Add("syntaxdon@gmail.com");
            mail.Subject = " Login Details for Exam Portal Cloud";

            mail.Body = " Dear" + "".PadRight(1) + student.Name + "" + " ,\n\n"
            + " Thank you for registering your account on Exam Portal Cloud.\n \n"
            + " Your Login Registration details: \n \n"
            + " ExamNo: " + student.ExamNo + "\n"
            + " Password: " + password + "\n \n"
            + " Use the following link to login : https://examportalcloud.co.za/ and select Student Login. \n \n "
            + " IMPORTANT: Please ensure you have Safe Exam Browser installed on your Windows / Mac computer/laptop. You may download it from here: https://sourceforge.net/projects/seb/files/seb/SEB_2.4.1/SafeExamBrowserInstaller.exe/download  If you do not have a Windows or a Mac book computer / laptop, you do not need to install Safe Exam Browser. \n \n"
            + " If you experience any problems during login or during your examination, please contact your exam invigilator immediately. \n \n"
            + " Kind Regards \n"
            + " The Exam Portal Cloud team";

            SmtpClient smtpServer = new()
            {
                Host = "smtp.gmail.com",
                Port = 587, 

                /*Host = "mail.smtp2go.com",
                Port = 2525,*/
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential("qiscmapp@gmail.com", "gkrikvoauqlshyzg"),
                //Credentials = new NetworkCredential("Support@v-soft.co.za", "*VSoft*2019"),
                Timeout = 20000
            };

            try
            {
                smtpServer.Send(mail);
                return true; 
                //return "Mail has been successfully sent!";
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
                // return "Fail Has error" + ex.Message;
            }

            //smtpServer.Send(mail);
            
        }

         //public async bool SendRegistrationEmail(Student student)
           public async Task<bool> SendRegistrationEmail(MailData student)
        {
            //if (student.EmailAddress is null) return;

            //var body = "Dear User: \n \n"
            //    + " Welcome to Exam Portal Cloud \n \n"
            //    + " Your Login Registration details: \n \n"
            //    + " Exam Number: " + student.ExamNo + "\n"
            //    + " Password: " + password + "\n \n"
            //    + " Use the following link to login : https://examportalcloud.co.za/ and select Student Login. \n \n "
            //    + " IMPORTANT: Please ensure you have Safe Exam Browser installed on your Windows / Mac computer/laptop. You may download it from here: https://sourceforge.net/projects/seb/files/seb/SEB_2.4.1/SafeExamBrowserInstaller.exe/download  If you do not have a Windows or a Mac book computer / laptop, you do not need to install Safe Exam Browser. \n \n"
            //    + " If you experience any problems during login or during your examination, please contact your exam invigilator immediately. \n \n"
            //    + " Kind Regards, \n"
            //    + " The Exam Portal Cloud team";

            //var emailDto = new EmailDto
            //{
            //    EmailAddesses = new List<string> { student.EmailAddress },
            //    MessageBody = body,
            //    Subject = "Login Details for Exam Portal Cloud",

            //};

            //_emailService.SendOrQueue(emailDto);
            MailMessage mail = new();
            //sending when emailing a studenton add student
            //mail.From = new MailAddress("qiscmapp@gmail.com");
            mail.From = new MailAddress("Support@v-soft.co.za");
            mail.To.Add("Tinashe@v-soft.co.za");
            mail.To.Add(student.EmailAddress);
            mail.Bcc.Add("support@v-soft.co.za");
            //mail.Bcc.Add("syntaxdon@gmail.com");
            mail.Subject = " Login Details for Exam Portal Cloud";

            mail.Body = " Dear" + "".PadRight(1) + student.Name + "" + " ,\n\n"
            + " Thank you for registering your account on Exam Portal Cloud.\n \n"
            + " Your Login Registration details: \n \n"
            + " ExamNo: " + student.ExamNo + "\n"
            + " Password: " + student.Password + "\n \n"
            + " Use the following link to login : https://examportalcloud.co.za/ and select Student Login. \n \n "
            + " IMPORTANT: Please ensure you have Safe Exam Browser installed on your Windows / Mac computer/laptop. You may download it from here: https://sourceforge.net/projects/seb/files/seb/SEB_2.4.1/SafeExamBrowserInstaller.exe/download  If you do not have a Windows or a Mac book computer / laptop, you do not need to install Safe Exam Browser. \n \n"
            + " If you experience any problems during login or during your examination, please contact your exam invigilator immediately. \n \n"
            + " Kind Regards \n"
            + " The Exam Portal Cloud team";

            SmtpClient smtpServer = new()
            {
                /* Host = "smtp.gmail.com",
                Port = 587,  */

                Host = "mail.smtp2go.com",
                Port = 2525,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Credentials = new NetworkCredential("qiscmapp@gmail.com", "gkrikvoauqlshyzg"),
                Credentials = new NetworkCredential("Support@v-soft.co.za", "*VSoft*2019"),
                Timeout = 20000
            };

            try
            {
                smtpServer.Send(mail);
                //return "Mail has been successfully sent!";
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                // return "Fail Has error" + ex.Message;
            }

            smtpServer.Send(mail);
            return smtpServer is not null;
            //return true; 
        }
        public async Task<Student> UpdateAsync(Student entity)
        {
            var student = await _repository.GetByIdAsync<Student>(entity.Id);
       /*     if (student is not null)
            {
                if (student.EncrytedPassword is not null)
                    entity.EncrytedPassword = student.EncrytedPassword;
                //entity.PlainPassword = PasswordHelper.Decrypt(student.EncrytedPassword, _examPortalSettings.EncryptionKey);
                //entity.EncrytedPassword = PasswordHelper.Encrypt(password, _examPortalSettings.EncryptionKey);
            } */



            return await _repository.UpdateAsync(entity, true);
        }
        public async Task PasswordMigration()
        {
            var parameters = new Dictionary<string, object>();
            var students = await _repository.ExecuteStoredProcAsync<Student>(StoredProcedures.PasswordMigration, parameters);
            try{
                foreach (var student in students)
                { 
                    if(student.Id == 9419){
                    /* if (student.PlainPassword != null)
                    { */
                        student.EncrytedPassword = PasswordHelper.Encrypt(student.PlainPassword, _examPortalSettings.EncryptionKey);
                        //student.EncrytedPassword = PasswordHelper.Encrypt("ED402DC7", _examPortalSettings.EncryptionKey);

                        await UpdateAsync(student);
                    }
                   /*  else{
                        throw new Exception(student.Id.ToString()); 
                    } */
                }
            }
            catch(Exception ex){
                //ex.Message +''+ students
                ex.Message.ToString();
            }
        }

         public Task<Student> AddAsync(Student entity)
        {
            throw new NotImplementedException();
        } 

        
    }
}
