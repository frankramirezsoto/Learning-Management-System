using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly LMSDbContext _dbContext;

        public EnrollmentRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Enrollment> GetByCompositeKeysAsync(int courseCycleId, int studentId)
        {
            var enrollment = await _dbContext.Enrollments
                .Include(e => e.CourseCycle)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.CourseCycleId == courseCycleId && e.StudentId == studentId);

            return enrollment;
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _dbContext.Enrollments
                .Include(e => e.CourseCycle)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetAllByCourseCycleIdAsync(int courseCycleId)
        {
            return await _dbContext.Enrollments
                .Include(e => e.Student)
                    .ThenInclude(e => e.Careers)
                        .ThenInclude(e => e.AcademicLevel)
                .Where(e => e.CourseCycleId == courseCycleId)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(Enrollment enrollment)
        {
            try
            {
                // Check if the enrollment already exists
                var existingEnrollment = await _dbContext.Enrollments
                    .FirstOrDefaultAsync(e =>
                        e.CourseCycleId == enrollment.CourseCycleId &&
                        e.StudentId == enrollment.StudentId);

                if (existingEnrollment != null)
                {
                    return (false, "Student is already enrolled.");
                }

                // If enrollment doesn't exist, add it to the database
                _dbContext.Enrollments.Add(enrollment);
                await _dbContext.SaveChangesAsync();

                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add enrollment: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Enrollment enrollment)
        {
            try
            {
                _dbContext.Enrollments.Update(enrollment);
                await _dbContext.SaveChangesAsync();
                return (true, "Enrollment updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update enrollment: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int courseCycleId, int studentId)
        {
            try
            {
                var enrollment = await GetByCompositeKeysAsync(courseCycleId, studentId);
                if (enrollment != null)
                {
                    _dbContext.Enrollments.Remove(enrollment);
                    await _dbContext.SaveChangesAsync();
                    return (true, "Enrollment deleted successfully.");
                }
                else
                {
                    return (false, "Enrollment not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete enrollment: {ex.Message}");
            }
        }
    }
}
