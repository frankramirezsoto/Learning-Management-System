using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Models.Entities
{
    public class Cycle
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool isCurrentCycle { get; set; }

        public List<Course>? Courses { get; } = [];
    }
}
