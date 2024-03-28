using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class EvaluationTaskRepository : IEvaluationTaskRepository
    {
        private readonly LMSDbContext _dbContext;

        public EvaluationTaskRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EvaluationTask> GetByIdAsync(int id)
        {
            return await _dbContext.EvaluationTasks
                .Include(e => e.EvaluationItem)
                .Include(e => e.Submissions)
                .FirstOrDefaultAsync(task => task.Id == id);
        }

        public async Task<IEnumerable<EvaluationTask>> GetAllByEvaluationItemIdAsync(int evaluationItemId)
        {
            return await _dbContext.EvaluationTasks
                .Include(e => e.EvaluationItem)
                .Include(e => e.Submissions)
                .Where(task => task.EvaluationItemId == evaluationItemId)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(EvaluationTask evaluationTask)
        {
            try
            {
                _dbContext.EvaluationTasks.Add(evaluationTask);
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add EvaluationTask: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(EvaluationTask evaluationTask)
        {
            try
            {
                var task = await _dbContext.EvaluationTasks.FindAsync(evaluationTask.Id);
                if (task != null) 
                { 
                    task.Name = evaluationTask.Name;
                    task.Description = evaluationTask.Description;
                    task.Points = evaluationTask.Points;
                    task.Expires = evaluationTask.Expires;
                }
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update EvaluationTask: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var task = await _dbContext.EvaluationTasks.FindAsync(id);
                if (task != null)
                {
                    _dbContext.EvaluationTasks.Remove(task);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "EvaluationTask not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete EvaluationTask: {ex.Message}");
            }
        }
    }

}
