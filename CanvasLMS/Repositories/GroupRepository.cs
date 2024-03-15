using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly LMSDbContext _dbContext;

        public GroupRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Group> GetByIdAsync(int? id, int courseCycleId)
        {
            return await _dbContext.Groups
                .Include(g => g.CourseCycle)
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id && g.CourseCycleId == courseCycleId);
        }

        public async Task<IEnumerable<Group>> GetAllByCourseCycleIdAsync(int courseCycleId)
        {
            return await _dbContext.Groups
                .Where(g => g.CourseCycleId == courseCycleId)
                .Include(g => g.Students)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(Group group)
        {
            try
            {
                _dbContext.Groups.Add(group);
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add Group: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Group group)
        {
            try
            {
                _dbContext.Groups.Update(group);
                await _dbContext.SaveChangesAsync();
                return (true, "200");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update Group: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id, int courseCycleId)
        {
            try
            {
                var group = await _dbContext.Groups.FindAsync(id, courseCycleId);
                if (group != null)
                {
                    _dbContext.Groups.Remove(group);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "Group not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete Group: {ex.Message}");
            }
        }
        //Function to add a Student to a Group 
        public async Task<(bool Success, string Message)> AddStudentToGroupAsync(int? groupId, int courseCycleId, int studentId)
        {
            try
            {
                //Gets both the user and the student by their Id
                var group = await _dbContext.Groups.FindAsync(groupId, courseCycleId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (group != null && student != null)
                {
                    if (group.Students != null)
                    {
                        group.Students.Add(student); //If Students List is not empty then it adds the Student to the list 
                    }
                    else 
                    { 
                        //Else creates a new Students List that will contain the added student 
                        var studentsListDTO = new List<Student>();
                        studentsListDTO.Add(student);
                        group.Students = studentsListDTO;
                    }
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                return (false, "Group or Student not found");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add to GroupStudent: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> RemoveStudentFromGroupAsync(int groupId, int courseCycleId, int studentId)
        {
            try
            {
                //Gets both the user and the student by their Id
                var group = await _dbContext.Groups.FindAsync(groupId, courseCycleId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (group != null && student != null)
                {
                    group.Students.Remove(student);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                return (false, "Group or Student not found");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add to GroupStudent: {ex.Message}");
            }
        }
    }
}
