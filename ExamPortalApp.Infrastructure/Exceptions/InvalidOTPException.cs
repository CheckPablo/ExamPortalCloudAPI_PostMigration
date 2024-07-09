using ExamPortalApp.Infrastructure.Constants;

namespace ExamPortalApp.Infrastructure;

//public class InvalidOTPException() : Exception(ErrorMessages.OTPEntryChecks.InvalidOTP)
public class InvalidOTPException(string message) : Exception(message)
{

}
