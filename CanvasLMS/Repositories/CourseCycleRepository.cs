using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class CourseCycleRepository : ICourseCycleRepository
    {
        //Receives Database Context with Dependency Injection
        protected readonly LMSDbContext _dbContext;

        public CourseCycleRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CourseCycle> GetByIdAsync(int id)
        {
            var CourseCycle = await _dbContext.CourseCycles
                .Include(cc => cc.Course)
                    .ThenInclude(co => co.Careers)
                .Include(cc => cc.Cycle)
                .Include(cc => cc.Professor)
                .Include(cc => cc.Classes)
                .Include(cc => cc.Enrollments)
                .Include(cc => cc.EvaluationItems)
                .Include(cc => cc.Groups)
                .FirstOrDefaultAsync(cc => cc.Id == id);

            return CourseCycle;
        }

        public async Task<IEnumerable<CourseCycle>> GetAllByProfessorIdAsync(int professorId)
        {
            return await _dbContext.CourseCycles
                .Include(cc => cc.Course)
                .Include(cc => cc.Cycle)
                .Include(cc => cc.Professor)
                .Include(cc => cc.Classes)
                .Include(cc => cc.Enrollments)
                .Include(cc => cc.EvaluationItems)
                .Include(cc => cc.Groups)
                .Where(cc => cc.ProfessorId == professorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseCycle>> GetAllByStudentIdAsync(int studentId)
        {
            return await _dbContext.CourseCycles
                .Include(cc => cc.Course)
                .Include(cc => cc.Cycle)
                .Include(cc => cc.Professor)
                .Include(cc => cc.Classes)
                .Include(cc => cc.Enrollments)
                .Include(cc => cc.EvaluationItems)
                .Include(cc => cc.Groups)
                .Where(cc => cc.Enrollments.Any(e => e.StudentId == studentId))
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseCycle>> GetAllByCourseIdAndCycleId(string courseId, string cycleId)
        {
            return await _dbContext.CourseCycles
                .Where(cc => cc.CourseId == courseId && cc.CycleId == cycleId)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(CourseCycle courseCycle)
        {
            try
            {
                _dbContext.CourseCycles.Add(courseCycle);
                await _dbContext.SaveChangesAsync();
                return (true, "CourseCycle added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add CourseCycle: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(CourseCycle courseCycle)
        {
            try
            {
                _dbContext.CourseCycles.Update(courseCycle);
                await _dbContext.SaveChangesAsync();
                return (true, "CourseCycle updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update CourseCycle: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var courseCycle = await _dbContext.CourseCycles.FindAsync(id);
                if (courseCycle != null)
                {
                    _dbContext.CourseCycles.Remove(courseCycle);
                    await _dbContext.SaveChangesAsync();
                    return (true, "CourseCycle deleted successfully.");
                }
                else
                {
                    return (false, "CourseCycle not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete CourseCycle: {ex.Message}");
            }
        }
    }
}

