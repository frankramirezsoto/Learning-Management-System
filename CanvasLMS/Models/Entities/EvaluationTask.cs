using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.Entities
{
    public class EvaluationTask
    {
        public int Id { get; set; }
        public int EvaluationItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(10,2)]
        public decimal Points { get; set; }
        public DateTime Published {  get; set; }
        public DateTime Expires { get; set; }

        public EvaluationItem? EvaluationItem { get; set; }
        public List<TaskSubmission>? Submissions { get; set; }
    }
}
