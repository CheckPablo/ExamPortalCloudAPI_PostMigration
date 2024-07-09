using ExamPortalApp.Infrastructure.Constants;

namespace ExamPortalApp.Infrastructure.Exceptions
{
    public class NotActiveException : Exception
    {
        public NotActiveException() : base(ErrorMessages.Auth.NotActive)
        {
        }
    }

    public class NotApprovedException : Exception
    {
        public NotApprovedException() : base(ErrorMessages.Auth.NotApproved)
        {
        }
    }

     public class InvalidGradeEntryException : Exception
    {
        public InvalidGradeEntryException() : base(ErrorMessages.GradeEntryChecks.GradeExists)
        {
        }
    }

    public class InvalidStudentEntryException: Exception{
        public InvalidStudentEntryException(): base(ErrorMessages.StudentEntryChecks.StudentExists)
        {
        }
    }
     public class InvalidSubjectEntryException: Exception{
        public InvalidSubjectEntryException(): base(ErrorMessages.SubjectEntryChecks.SubjectExists)
        {
        }
    }
     

     public class InvalidUserNameException : Exception
    {
        public InvalidUserNameException() : base(ErrorMessages.Auth.UserNameNonExistant)
        {
        }
    }

       public class InvalidPasswordException: Exception
    {
        public InvalidPasswordException() : base(ErrorMessages.Auth.InvalidCredentials)
        {
        }
    }


    public class LicenseExipredException : Exception
    {
        public LicenseExipredException() : base(ErrorMessages.Auth.ExpiredLicense)
        {
        }
    }
    public class InvalidCrdentialsException : Exception
    {
        public InvalidCrdentialsException() : base(ErrorMessages.Auth.InvalidCredentials)
        {
        }
    }
}
