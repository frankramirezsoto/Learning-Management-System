using CanvasLMS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.ViewModels
{
    public class TaskSubmissionViewModel
    {
        public int? Id { get; set; }
        public int EvaluationTaskId { get; set; }
        public int StudentId { get; set; }
        public string? FilePath { get; set; }
        public DateTime? SubmissionDate { get; set; }
        [Precision(10, 2)]
        public decimal? Score { get; set; }

        public EvaluationTask? EvaluationTask { get; set; }
        public Student? Student { get; set; }
    }
}
