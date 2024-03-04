using CanvasLMS.Data;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class UserRepository<TEntity> : IUserRepository<TEntity> where TEntity : class
    {
        //Receives Database Context with Dependency Injection
        protected readonly LMSDbContext _dbContext;

        public UserRepository(LMSDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Methods to get the Entity's Information by Id or Email 
        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity?> GetByEmailAsync(string email)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e => EF.Property<string>(e, "Email") == email);
        }
        //Method to get all users
        public async Task<IEnumerable<TEntity>>? GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }
        //Method to authenticate
        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            var entity = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(e =>
                EF.Property<string>(e, "Email") == email &&
                EF.Property<string>(e, "Password") == password);

            return entity != null;
        }
        //CRUD Methods
        public async Task<(bool Success, string Message)> AddAsync(TEntity entity)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return (true, $"{typeof(TEntity).Name} added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to add {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UpdateAsync(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
                await _dbContext.SaveChangesAsync();
                return (true, $"{typeof(TEntity).Name} updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to update {typeof(TEntity).Name}: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteAsync(int id)
        {
            try
            {
                var entity = await _dbContext.Set<TEntity>().FindAsync(id);
                if (entity != null)
                {
                    _dbContext.Set<TEntity>().Remove(entity);
                    await _dbContext.SaveChangesAsync();
                    return (true, $"{typeof(TEntity).Name} deleted successfully.");
                }
                else
                {
                    return (false, $"{typeof(TEntity).Name} not found.");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete {typeof(TEntity).Name}: {ex.Message}");
            }
        }
    }
}


