using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> GetByCompositeKeysAsync(int courseCycleId, int studentId);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetAllByCourseCycleIdAsync(int courseCycleId);
        Task<(bool Success, string Message)> AddAsync(Enrollment enrollment);
        Task<(bool Success, string Message)> UpdateAsync(Enrollment enrollment);
        Task<(bool Success, string Message)> DeleteAsync(int courseCycleId, int studentId);
    }
}
