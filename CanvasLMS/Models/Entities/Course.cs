using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanvasLMS.Models.Entities
{
    public class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Cycle> Cycles { get; } = [];
        public List<Career> Careers { get; } = [];
    }
}
