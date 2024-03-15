using CanvasLMS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.ViewModels
{
    public class EvaluationItemViewModel
    {
        public int Id { get; set; }

        public int CourseCycleId { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Percentage { get; set; }
        public bool IsGroupal { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public List<EvaluationTask>? Tasks { get; set; }
    }
}
