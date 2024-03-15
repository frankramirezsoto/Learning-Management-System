using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class EvaluationItemRepository : IEvaluationItemRepository
    {
        private readonly LMSDbContext _dbContext;

        public EvaluationItemRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EvaluationItem> GetByIdAsync(int id)
        {
            var evaluationItem = await _dbContext.EvaluationItems
                .Include(e => e.CourseCycle)
                .Include(e => e.Tasks)
                    .ThenInclude(e => e.Submissions)
                        .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(e => e.Id == id);
            return evaluationItem;
        }

        public async Task<IEnumerable<EvaluationItem>> GetAllByCourseCycleIdAsync(int courseCycleId)
        {
            return await _dbContext.EvaluationItems
                .Include(e => e.CourseCycle)
                .Include(e => e.Tasks)
                    .ThenInclude(e => e.Submissions)
                        .ThenInclude(e => e.Student)
                .Where(e => e.CourseCycleId == courseCycleId)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(EvaluationItem evaluationItem)
        {
            try
            {
                //Validation used to check if the percentage being added makes the note higher than 100 
                var existingItems = await GetAllByCourseCycleIdAsync(evaluationItem.CourseCycleId);
                decimal totalPercentage = existingItems.Sum(item => item.Percentage) + evaluationItem.Percentage;

                if (totalPercentage <= 100)
                {
                    _dbContext.EvaluationItems.Add(evaluationItem);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "The percentage you are trying to add makes the final note exceed 100%.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add EvaluationItem: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(EvaluationItem evaluationItem)
        {
            try
            {
                //Validation used to check if the percentage being added makes the note higher than 100 
                var existingItems = await GetAllByCourseCycleIdAsync(evaluationItem.CourseCycleId);
                decimal totalPercentage = existingItems.Sum(item => item.Id == evaluationItem.Id ? 0 : item.Percentage) + evaluationItem.Percentage;

                if ((totalPercentage) <= 100)
                {
                    var evaluationItemDTO = await _dbContext.EvaluationItems.FindAsync(evaluationItem.Id);
                    evaluationItemDTO.Name = evaluationItem.Name;
                    evaluationItemDTO.Description = evaluationItem.Description;
                    evaluationItemDTO.Percentage = evaluationItem.Percentage;
                    evaluationItemDTO.IsGroupal = evaluationItem.IsGroupal;

                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "The percentage you are trying to add makes the final note exceed 100%.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update Evaluation Item: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var evaluationItem = await _dbContext.EvaluationItems.FindAsync(id);
                if (evaluationItem != null)
                {
                    _dbContext.EvaluationItems.Remove(evaluationItem);
                    await _dbContext.SaveChangesAsync();
                    return (true, "200");
                }
                else
                {
                    return (false, "Evaluation Item not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete Evaluation Item: {ex.Message}");
            }
        }
    }
}
