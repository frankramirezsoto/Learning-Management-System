using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.Entities
{
    public class TaskSubmission
    {
        public int Id { get; set; }
        public int EvaluationTaskId { get; set; }
        public int StudentId { get; set; }
        public string FilePath { get; set; }
        [Precision(3, 2)]
        public decimal? Score { get; set; }

        public EvaluationTask? EvaluationTask { get; set; }
        public Student? Student { get; set; }
    }
}
