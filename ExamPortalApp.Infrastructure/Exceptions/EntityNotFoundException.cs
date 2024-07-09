using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Infrastructure.Constants;

namespace ExamPortalApp.Infrastructure.Exceptions
{
    public class EntityNotFoundException<T>(int id) : Exception(string.Format(ErrorMessages.EntityNotFound, id, typeof(T))) where T : EntityBase
    {
    }
}
