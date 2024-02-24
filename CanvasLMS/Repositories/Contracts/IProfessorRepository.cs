using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IProfessorRepository : IUserRepository<Professor>
    {
        Task<IEnumerable<Professor>> GetProfessorsByCareer(int careerId);
        Task LoadProfessorRelationshipsAsync(Professor professor);
    }
}
