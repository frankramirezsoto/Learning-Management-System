namespace CanvasLMS.Repositories.Contracts
{
    public interface IUserRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync(int id);
        Task<TEntity?> GetByEmailAsync(string email);
        Task<IEnumerable<TEntity>>? GetAllAsync();
        Task<bool> AuthenticateAsync(string email, string password);
        Task<(bool Success, string Message)> AddAsync(TEntity entity);
        Task<(bool Success, string Message)> UpdateAsync(TEntity entity);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
