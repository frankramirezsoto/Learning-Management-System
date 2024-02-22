using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class StudentRepository : UserRepository<Student>, IStudentRepository
    {
        public StudentRepository(LMSDbContext dbContext) : base(dbContext) { }

        public async Task LoadStudentRelationshipsAsync(Student student)
        {
            // Load related data for Student
            await _dbContext.Entry(student)
                .Collection(s => s.Attendance)
                .LoadAsync();

            await _dbContext.Entry(student)
                .Collection(s => s.Enrollments)
                .LoadAsync();

            await _dbContext.Entry(student)
                .Collection(s => s.Scores)
                .LoadAsync();

            await _dbContext.Entry(student)
                .Collection(s => s.Groups)
                .LoadAsync();
        }
    }
}
