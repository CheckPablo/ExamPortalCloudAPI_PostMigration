namespace ExamPortalApp.Infrastructure.Constants
{
    internal static class ErrorMessages
    {
        internal static readonly string EntityNotFound = "The specified entity with Id '{0}' of type '{1}' was not found.";
        internal static readonly string FileIsRequired = "The file is required";
        internal static readonly string RequiredDocumentNotFound = "Required document could not be found";
        internal static readonly string InvalidRequiredDocument = "The selected required document is invalid";


        internal static class Auth
        {
            internal static readonly string NotActive = "Your account is not active";
            internal static readonly string ExpiredLicense = "Your License has expired";
            internal static readonly string NotApproved = "Your account has not yet been approved";
            internal static readonly string InvalidCredentials = "The user name or password provided is incorrect."; 
            internal static readonly string UserNameNonExistant = "The user name provided does not exist"; 
            internal static readonly string Unauthorised = "Not authorised";
            internal static readonly string UserNotRegitered = "User could not be created.";
            internal static readonly string UsernameExists = "The specified username already exists";
            internal static readonly string MicrosoftLogin = "Use Microsoft Login";
            internal static readonly string PasswordLength = "Password Length must be greater than 8 characters";
            internal static readonly string PasswordSpecialCharacter = "Password must contain at least one special character";
            internal static readonly string PasswordUpperCase = "Password must contain at least one upper case character";
            internal static readonly string PasswordValidationMessage = "Password must contain more than 8 characters, 1 upper case letter, and 1 special character";
            internal static readonly string CenterMissing = "Missing Center";
            internal static readonly string StudentDoesNotExist = "Student does not exist";

            //public static string? InvalidCrdentials { get; internal set; }
        }

        internal static class GradeEntryChecks
        { 
            // more grade checks will be added here 
            internal static readonly string GradeExists = "The specified grade code and description already exists";
        }
        internal static class SubjectEntryChecks
        {
            // more subject checks will be added here 
            internal static readonly string SubjectExists = "The specified subject code and subject name already exists";
        }

        internal static class StudentEntryChecks
        {
            // more student checks will be added here 
            internal static readonly string StudentExists = "The specified student number already exists";
        }

        internal static class TestEntryChecks
        {
            // more student checks will be added here 
            internal static readonly string TestExists = "The specified test name already exists for this grade";
        }

        internal static class CenterEntryChecks
        {
            internal static readonly string CenterExists = "The specified center name already exists";
        }
        internal class OTPEntryChecks{
            internal static readonly string InvalidOTP = "Invalid OTP";
            internal static readonly string ExpiredOTP = "Expired OTP";
        }
    }
}
