using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IStudentRepository : IUserRepository<Student>
    {
        Task LoadStudentRelationshipsAsync(Student student);

        Task<(bool Success, string Message)> AddNewStudent(Student student);
    }
}
