namespace ExamPortalApp.Infrastructure.Constants
{
    internal static class StoredProcedures
    {
        internal const string AuthUser = "[dbo].[Login_AuthenticateUser]";
        internal const string AuthStudent = "[dbo].[Login_AuthenticateStudent]";
        internal const string PasswordMigration = "[dbo].[PasswordMigration]";
        internal const string InvigilatorStudentLinkGet = "[dbo].[InvigilatorStudentLink_get]"; // NOT YET IN DB
        // NOT YET IN DB
        internal const string StudentTestLinkGet = "GetStudentList_testSettings";
        internal const string GetTestsOTP  = "[dbo].[Get_OTP]";
        internal const string NewOTPInsert = "[dbo].[RNDOTP_ins1]"; 

        internal const string AddStudentTemp = "[dbo].[RNDOTP_ins1]"; 
        internal const string OtpEmailList = "[dbo].[otpEmailList1]";
        internal const string OtpToSend = "[dbo].[Get_OTPToSend]"; // NOT YET IN DB
        internal const string StudentTestLinkInsert = "[dbo].[StudentTestLink_ins1]";
        internal const string UserApprovalState = "[dbo].[zGetAllUser_modified]";
        internal const string StudentTestList = "[dbo].[Get_Test]";
        internal const string LinkStudent = "LinkStudent";
        internal const string ResetTestLinkStundent = "ResetTestLinkStundent";
        internal const string GetAttendanceRegister = "[dbo].[Get_AttendanceRegister]";
        internal const string SetAttendanceRegister = "[dbo].[SetAttendanceRegister]"; 
        internal const string ResetTestForStudent = "[dbo].[ResetTestForStudent]";
        internal const string GetCenterAttendance = "[dbo].[Get_CentreAttendance]";
        internal const string GetCurrentCenterDetails = "[dbo].[Get_CurrentCenterDetails]"; 
        internal const string updateTestStartDateTime = "[dbo].[upTestStartDateTime_TestUpload]";
        internal const string Get_ConfirmTestOTPExist = "Get_ConfirmTestOTPExist";
        internal const string AcceptDisclaimerInsert = "DisclaimerAccept_ins";
        internal const string LiveMonitoringCanidateList = "LiveMonitoringCanidateList";
        internal const string LiveMonitoringKeypressTracking = "LiveMonitoringKeypressTracking";
        internal const string StudentTestAnswersIntervalSave = "IntervalSave"; // NOT IN DB 
       internal const string StudentMaintenanceDelete = "StudentMaintenance_delete";
        internal const string StudentMaintenance_InsertUpdate_Revised = "StudentMaintenance_InsertUpdate_Revised";
        internal const string KeyPressTracking_ins = "KeyPressTracking_ins";
        internal const string LiveMonitoringIrregularities = "LiveMonitoringIrregularities";
        internal static readonly string UserApproval = "UserApproval";
        internal static readonly string LoadTestOnExamStart ="LoadTestOnExamStart";
        internal static string LiveMonitoringStudentAnswerProgress = "LiveMonitoringStudentAnswerProgress";
        internal static string GetStudentTestDetails = "get_StudentTestDetails";
        internal static string InsertExtraTimeMonitoring = "Insert_ExtraTime_Monitoring";
        internal static string SaveStudentAnswer_TestUpload = "SaveStudentAnswer_TestUpload";
        internal static string DeleteStudentSubjects = "DeleteStudentSubjects";
        internal static string LinkStudentSubjects = "LinkStudentSubjects";
        internal static string Get_CenterSummary = "Get_CenterSummary";
        internal static string SubjectMaintenance_InsUpd = "SubjectMaintenance_InsUpd";
        internal static string get_StudentAnswersList = "get_StudentAnswersList";

        internal static string get_StudentAnswersList_Export = "get_StudentAnswersList_Export";
        internal static string CompleteTest = "CompleteTest";
        internal static string UploadScannedImageDetails = "UploadScannedImageDetails"; 
        internal static string VerifyScannedImagesOTP = "VerifyScannedImagesOTP"; 
        internal static string BulkImportExamPortalCloud = "BulkImportExamPortalCloud";
        internal static string BulkImportExamPortalCloudEdited = "BulkImportExamPortalCloudEdited";
        internal static string  ImportSubjectSectors = "ImportSubjectSectors"; 
        internal static string  ImportPeople  = "ImportPeople";
        internal static string  ImportPeople_v2  = "ImportPeople_v2";
        internal static string  ImportLink  = "ImportLink";
        internal static string  Get_BulkImport_BatchID  = "Get_BulkImport_BatchID";
         internal static string GetBatchID_BulkImport = "GetBatchID_BulkImport"; 
         internal static string UpdateStudentCredentials = "UpdateStudentCredentials";
         internal static string ApproveUsers = "ApproveUsers"; 
         internal static string insertUpdateTestQuestionPaper = "insertUpdateTestQuestionPaper";
         internal static string retrieveQuestionPaper = "retrieveQuestionPaper";

         internal static string insertUpdateAnswerPaper= "insertUpdateAnswerPaper";
       
        internal static class Params
        {
            internal static readonly string Email = "Email";

            internal static readonly string ExternalEmail = "ExternalEmail";
             internal static readonly string ContactNo = "ContactNo";
            internal static readonly string ContactDetails = "ContactDetails";
            internal static readonly string CenterID = "CenterID";
            internal static readonly string GradeId = "GradeId";
            internal static readonly string InvigilatorId = "InvigilatorId";
            internal static readonly string Name = "Name";
            internal static readonly string Password = "Password";
            internal static readonly string pLoginName = "pLoginName";
            internal static readonly string pPassword = "pPassword";
            internal static readonly string RoleID = "RoleID";
            internal static readonly string SubjectId = "SubjectId";
            internal static readonly string Surname = "Surname";
            internal static readonly string Username = "Username";
            internal static readonly string TestID = "TestID";
            internal static readonly string SectorId = "SectorId";
            internal static readonly string Code = "Code";
            internal static readonly string Description = "Description";
            internal static readonly string ModifiedDate = "ModifiedDate";
            internal static readonly string active = "active";
            internal static readonly string approve = "approve";
            internal static readonly string id = "id";
            internal static readonly string StudentId = "StudentId";
            internal static readonly string EligibleForExternalLogin = "EligibleForExternalLogin";
            internal static readonly string Absent = "Absent";
            internal static readonly string ExamDate = "ExamDate";
            internal static readonly string EndExamDate = "EndExamDate";
            internal static readonly string OTP = "OTP";
            internal const string PasswordMigration = "[dbo].[PasswordMigration]";
            //internal static readonly string OTP = "OTP";
            internal static readonly bool Accepted = false;
            internal static string CandidateSearchType = "CandidateSearchType";
            internal static string BatchID = "BatchID";
            internal static readonly string TimeRemaining = "TimeRemaining";
            internal static readonly string KeyPress = "KeyPress";
            internal static readonly string LeftExamArea = "LeftExamArea";
            internal static readonly string Offline = "Offline";
            internal static readonly string FullScreenClosed = "FullScreenClosed";
           
            internal static readonly string FileName = "FileName";
             internal static readonly string FilePath = "FilePath";
            //internal static varbinary TestDocument = "";
            internal static readonly string AnswerText = "AnswerText";
            internal static readonly string Accomodation = "Accomodation";
            internal static readonly string StudentExtraTime = "StudentExtraTime";
            internal static readonly string ModifiedBy = "ModifiedBy";

            internal static readonly string EncryptedPassword = "EncryptedPassword";
            internal static readonly string IsActive = "Active";
            internal static readonly string IsApproved = "Approve";
            internal static readonly string IsAdmin = "Admin" ;
            internal static readonly string UserId = "UserId";
            internal static readonly string ElectronicReader = "ElectronicReader";
            internal static string TestDocument = "TestDocument";
            internal static string StudentSubjectID = "StudentSubjectID";
            internal static string Event = "Event";
            internal static string Reason = "Reason";
            internal static string RegionId = "RegionId";
            internal static string CertLangID = "CertLangId";
            internal static string ExpiryDate = "ExpiryDate";
            internal static string Batch = "Batch";
            internal static string SectorCode = "SectorCode";
            internal static string Sector ="Sector";
            internal static string SubjectCode ="SubjectCode";
            internal static string Subject ="Subject";
            internal static string StudentNo ="StudentNo";
            internal static string  CenterNo  ="CenterNo";
            internal static string RegionID  ="RegionID";
            internal static string IDNumber  ="IDNumber";
            internal static string CellPhone ="CellPhone";
            internal static string BlankParam = "BlankParam";
            public static string TestTypeId = "TestTypeId";
            public static string TestCategoryId ="testCategoryId";
            public static string LanguageId = "LanguageId";
            public static string PaperExpiryDate = "PaperExpiryDate";
            public static string TestDuration = "TestDuration";               
            public static string Tts = "Tts";
            public static string TestName = "TestName";
            public static string WorkOffline = "WorkOffline";
            public static string AnswerScanningAvailable = "AnswerScanningAvailable";
            public static string TestSecurityLevelId = "TestSecurityLevelId";

        }

        internal static class Responses
        {
            internal const string LoggedIn = "User successfully logged in";
            internal const string OTPUpdated = "OTP Updated successfully";
            internal const string OTPSet = "OTP Set Successfully";
        }
    }
}
