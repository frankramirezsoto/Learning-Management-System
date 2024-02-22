using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class ProfessorRepository : UserRepository<Professor>, IProfessorRepository
    {
        public ProfessorRepository(LMSDbContext dbContext) : base(dbContext) { }

        public async Task LoadProfessorRelationshipsAsync(Professor professor)
        {
            await _dbContext.Entry(professor)
                .Collection(p => p.CourseCycles)
                .LoadAsync();

            await _dbContext.Entry(professor)
                .Collection(p => p.Classes)
                .LoadAsync();
        }
    }
}

