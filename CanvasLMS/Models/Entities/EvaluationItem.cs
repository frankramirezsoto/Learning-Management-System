using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanvasLMS.Models.Entities
{
    public class EvaluationItem
    {
        public int Id { get; set; }

        public int CourseCycleId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        [Precision(3,2)]
        public decimal Percentage { get; set; }
        public bool IsGroupal { get; set; }

        public CourseCycle CourseCycle { get; set; }
        public List<Score> Scores { get; set; }
    }
}
