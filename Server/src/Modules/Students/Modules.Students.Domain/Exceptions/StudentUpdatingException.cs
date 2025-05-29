namespace Modules.Students.Domain.Exceptions
{
    public class StudentUpdatingException(string id)
        : Exception($"Student with id: {id} update error");
}