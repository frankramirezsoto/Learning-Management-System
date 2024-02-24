using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface ICourseRepository
    {
        Task<Course> GetByIdAsync(string id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetCoursesByCareerIdAsync(int careerId);
        Task<(bool Success, string Message)> AddAsync(Course course);
        Task<(bool Success, string Message)> UpdateAsync(Course course);
        Task<(bool Success, string Message)> DeleteAsync(string id);
    }
}
