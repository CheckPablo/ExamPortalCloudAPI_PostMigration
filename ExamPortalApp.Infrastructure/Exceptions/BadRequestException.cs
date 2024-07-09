namespace ExamPortalApp.Infrastructure;

public class BadRequestException(string message) : Exception(message)
{
}
