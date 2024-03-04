using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetAllByCourseCycleIdAsync(int courseCycleId);
        Task<(bool Success, string Message)> AddAsync(Group group);
        Task<(bool Success, string Message)> UpdateAsync(Group group);
        Task<(bool Success, string Message)> DeleteAsync(int id, int courseCycleId);
        Task<(bool Success, string Message)> AddStudentToGroupAsync(int groupId, int courseCycleId, int studentId);
        Task<(bool Success, string Message)> RemoveStudentFromGroupAsync(int groupId, int courseCycleId, int studentId);
    }
}
