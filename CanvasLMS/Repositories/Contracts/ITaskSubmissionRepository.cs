using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface ITaskSubmissionRepository
    {
        Task<TaskSubmission> GetByIdAsync(int id);
        Task<List<TaskSubmission>> GetAllByEvaluationTaskIdAsync(int evaluationTaskId);
        Task<(bool Success, string Message)> AddAsync(TaskSubmission taskSubmission);
        Task<(bool Success, string Message)> UpdateAsync(TaskSubmission taskSubmission);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<(bool Success, string Message)> UpdateScoreAsync(int id, decimal? score);
    }
}
