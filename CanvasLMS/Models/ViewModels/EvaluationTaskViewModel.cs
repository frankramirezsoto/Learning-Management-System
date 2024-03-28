using CanvasLMS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.ViewModels
{
    public class EvaluationTaskViewModel
    {
        public int? Id { get; set; }
        public int? EvaluationItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Points { get; set; }
        public DateTime? Published { get; set; }
        public DateTime Expires { get; set; }

        public EvaluationItem? EvaluationItem { get; set; }
        public List<TaskSubmission>? Submissions { get; set; }
    }
}
