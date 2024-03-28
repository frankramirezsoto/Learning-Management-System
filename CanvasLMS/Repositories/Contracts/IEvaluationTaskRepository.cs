using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IEvaluationTaskRepository
    {
        Task<EvaluationTask> GetByIdAsync(int id);
        Task<IEnumerable<EvaluationTask>> GetAllByEvaluationItemIdAsync(int evaluationItemId);
        Task<(bool Success, string Message)> AddAsync(EvaluationTask evaluationTask);
        Task<(bool Success, string Message)> UpdateAsync(EvaluationTask evaluationTask);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
