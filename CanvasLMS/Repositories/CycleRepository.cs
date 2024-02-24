using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class CycleRepository : ICycleRepository
    {
        private readonly LMSDbContext _dbContext;

        public CycleRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Cycle> GetByIdAsync(int id)
        {
            return await _dbContext.Cycles.FindAsync(id);
        }

        public async Task<IEnumerable<Cycle>> GetAllAsync()
        {
            return await _dbContext.Cycles.ToListAsync();
        }

        public async Task<Cycle> GetCurrentCycleAsync()
        {
            return await _dbContext.Cycles.FirstOrDefaultAsync(c => c.isCurrentCycle);
        }

        public async Task<(bool Success, string Message)> AddAsync(Cycle cycle)
        {
            try
            {
                _dbContext.Cycles.Add(cycle);
                await _dbContext.SaveChangesAsync();
                return (true, "Cycle added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add Cycle: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Cycle cycle)
        {
            try
            {
                _dbContext.Cycles.Update(cycle);
                await _dbContext.SaveChangesAsync();
                return (true, "Cycle updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update Cycle: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var cycle = await _dbContext.Cycles.FindAsync(id);
                if (cycle != null)
                {
                    _dbContext.Cycles.Remove(cycle);
                    await _dbContext.SaveChangesAsync();
                    return (true, "Cycle deleted successfully.");
                }
                else
                {
                    return (false, "Cycle not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete Cycle: {ex.Message}");
            }
        }
    }
}
