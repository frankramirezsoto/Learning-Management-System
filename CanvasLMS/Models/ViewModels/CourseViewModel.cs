using CanvasLMS.Models.Entities;

namespace CanvasLMS.Models.ViewModels
{
    public class CourseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Cycle>? Cycles { get; } = [];
        public List<Career>? Careers { get; } = [];
    }
}
