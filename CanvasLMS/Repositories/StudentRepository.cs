using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using CanvasLMS.Utilities;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class StudentRepository : UserRepository<Student>, IStudentRepository
    {
        public StudentRepository(LMSDbContext dbContext) : base(dbContext) { }

        public async Task<(bool Success, string Message)> AddNewStudent(Student student)
        {
            // Generate a random secure password
            string temporaryPassword = PasswordUtility.GenerateRandomPassword(8);
            // Set the temporary password for the student
            student.Password = temporaryPassword;

            var add = await AddAsync(student);

            return add;
        }

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
                .Collection(s => s.Groups)
                .LoadAsync();
        }
    }
}
