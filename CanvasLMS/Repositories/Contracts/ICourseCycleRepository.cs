using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface ICourseCycleRepository
    {
        Task<CourseCycle> GetByIdAsync(int id);
        Task<IEnumerable<CourseCycle>> GetAllByProfessorIdAsync(int id);
        Task<IEnumerable<CourseCycle>> GetAllByStudentIdAsync(int id);
        Task<IEnumerable<CourseCycle>> GetAllByCourseIdAndCycleId(string CourseId, string CycleId);
        Task<(bool Success, string Message)> AddAsync(CourseCycle courseCycle);
        Task<(bool Success, string Message)> UpdateAsync(CourseCycle courseCycle);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
