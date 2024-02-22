using CanvasLMS.Models.Entities;

namespace CanvasLMS.Repositories.Contracts
{
    public interface IProfessorRepository : IUserRepository<Professor>
    {
        Task LoadProfessorRelationshipsAsync(Professor professor);
    }
}
