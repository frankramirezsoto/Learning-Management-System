using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface ICareerRepository
    {
        Task<Career> GetByIdAsync(int id);
        Task<IEnumerable<Career>> GetAllAsync();
        Task<(bool Success, string Message)> AddAsync(Career career);
        Task<(bool Success, string Message)> UpdateAsync(Career career);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
