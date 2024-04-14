using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class TaskSubmissionRepository : ITaskSubmissionRepository
    {
        private readonly LMSDbContext _dbContext;

        public TaskSubmissionRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskSubmission> GetByIdAsync(int id)
        {
            return await _dbContext.TaskSubmissions
                .Include(c => c.Student)
                .Include(c => c.EvaluationTask)
                    .ThenInclude(c => c.EvaluationItem)
                .FirstOrDefaultAsync(submission => submission.Id == id);
        }

        public async Task<List<TaskSubmission>> GetAllByEvaluationTaskIdAsync(int evaluationTaskId)
        {
            return await _dbContext.TaskSubmissions
                .Where(submission => submission.EvaluationTaskId == evaluationTaskId)
                .Include(c => c.Student)
                .Include(c => c.EvaluationTask)
                    .ThenInclude(c => c.EvaluationItem)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(TaskSubmission taskSubmission)
        {
            try
            {
                _dbContext.TaskSubmissions.Add(taskSubmission);
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add TaskSubmission: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(TaskSubmission taskSubmission)
        {
            try
            {
                _dbContext.TaskSubmissions.Update(taskSubmission);
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update TaskSubmission: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var submission = await _dbContext.TaskSubmissions.FindAsync(id);
                if (submission != null)
                {
                    _dbContext.TaskSubmissions.Remove(submission);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "Task Submission not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete TaskSubmission: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateScoreAsync(int id, decimal? score)
        {
            try
            {
                var submission = await _dbContext.TaskSubmissions.FindAsync(id);
                submission.Score = score;
                await _dbContext.SaveChangesAsync();

                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update TaskSubmission: {ex.Message}");
            }
        }
    }
}
