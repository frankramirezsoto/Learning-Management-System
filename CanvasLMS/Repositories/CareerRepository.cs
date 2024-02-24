using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class CareerRepository : ICareerRepository
    {
        private readonly LMSDbContext _dbContext;

        public CareerRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Career> GetByIdAsync(int id)
        {
            var CareersList = await _dbContext.Careers
                .Include(c => c.AcademicLevel)
                .Include(c => c.Faculty)
                .FirstOrDefaultAsync(c => c.Id == id);
            return CareersList;
        }

        public async Task<IEnumerable<Career>> GetAllAsync()
        {
            return await _dbContext.Careers
                .Include(c => c.AcademicLevel)
                .Include(c => c.Faculty)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> AddAsync(Career career)
        {
            try
            {
                _dbContext.Careers.Add(career);
                await _dbContext.SaveChangesAsync();
                return (true, "Career added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add Career: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(Career career)
        {
            try
            {
                _dbContext.Careers.Update(career);
                await _dbContext.SaveChangesAsync();
                return (true, "Career updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update Career: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var career = await _dbContext.Careers.FindAsync(id);
                if (career != null)
                {
                    _dbContext.Careers.Remove(career);
                    await _dbContext.SaveChangesAsync();
                    return (true, "Career deleted successfully.");
                }
                else
                {
                    return (false, "Career not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete Career: {ex.Message}");
            }
        }
    }
}
