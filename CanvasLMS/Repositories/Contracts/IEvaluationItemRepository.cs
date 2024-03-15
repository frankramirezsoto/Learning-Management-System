using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IEvaluationItemRepository
    {
        Task<EvaluationItem> GetByIdAsync(int id);
        Task<IEnumerable<EvaluationItem>> GetAllByCourseCycleIdAsync(int courseCycleId);
        Task<(bool Success, string Message)> AddAsync(EvaluationItem evaluationItem);
        Task<(bool Success, string Message)> UpdateAsync(EvaluationItem evaluationItem);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
