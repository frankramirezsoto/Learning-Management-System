using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LMSDbContext _dbContext;

        public CourseRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Course> GetByIdAsync(string id)
        {
            var CoursesList = await _dbContext.Courses
                .Include(c => c.Cycles)
                .Include(c => c.Careers)
                .FirstOrDefaultAsync(c => c.Id == id);
            return CoursesList;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _dbContext.Courses
                .Include(c => c.Cycles)
                .Include(c => c.Careers)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByCareerIdAsync(int careerId)
        {
            var courses = await _dbContext.Courses
                .Where(c => c.Careers.Any(career => career.Id == careerId))
                .ToListAsync();

            return courses;
        }
        public async Task<(bool Success, string Message)> AddAsync(Course course)
        {
            try
            {
                _dbContext.Courses.Add(course);
                await _dbContext.SaveChangesAsync();
                return (true, "Course added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add Course: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Course course)
        {
            try
            {
                _dbContext.Courses.Update(course);
                await _dbContext.SaveChangesAsync();
                return (true, "Course updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update Course: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(string id)
        {
            try
            {
                var course = await _dbContext.Courses.FindAsync(id);
                if (course != null)
                {
                    _dbContext.Courses.Remove(course);
                    await _dbContext.SaveChangesAsync();
                    return (true, "Course deleted successfully.");
                }
                else
                {
                    return (false, "Course not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete Course: {ex.Message}");
            }
        }
    }
}
