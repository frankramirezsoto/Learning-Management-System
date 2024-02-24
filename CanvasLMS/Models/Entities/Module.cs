using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(Id), nameof(CourseCycleId))]
    public class Module
    {
        public int Id { get; set; }
        public int CourseCycleId { get; set; }
        public string? Name { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public List<ModulePath>? ModulePaths { get; set; }
    }
}
