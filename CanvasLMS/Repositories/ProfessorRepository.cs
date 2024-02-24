using CanvasLMS.Data;
using CanvasLMS.Models.Entities;
using CanvasLMS.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Repositories
{
    public class ProfessorRepository : UserRepository<Professor>, IProfessorRepository
    {
        public ProfessorRepository(LMSDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Professor>> GetProfessorsByCareer(int careerId)
        {
            var career = await _dbContext.Careers
            .Include(c => c.Professors)
            .FirstOrDefaultAsync(c => c.Id == careerId);

            return career?.Professors.ToList() ?? new List<Professor>();
        }

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

