

﻿using ExamPortalApp.Infrastructure.Constants;
namespace ExamPortalApp.Infrastructure;

public class ExpiredOTPException(): Exception(ErrorMessages.OTPEntryChecks.ExpiredOTP)
{

}
