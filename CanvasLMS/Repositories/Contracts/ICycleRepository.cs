using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface ICycleRepository
    {
        Task<Cycle> GetByIdAsync(int id);
        Task<IEnumerable<Cycle>> GetAllAsync();
        Task<Cycle> GetCurrentCycleAsync();
        Task<(bool Success, string Message)> AddAsync(Cycle cycle);
        Task<(bool Success, string Message)> UpdateAsync(Cycle cycle);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
