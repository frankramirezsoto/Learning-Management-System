using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(StudentId), nameof(EvaluationItemId))]
    public class Score
    {
        public int StudentId { get; set; }
        public int EvaluationItemId { get; set; }
        [Precision(3, 2)]
        public decimal ScorePercentage { get; set; }

        public EvaluationItem EvaluationItem { get; set; }
        public Student Student { get; set; }
    }
}
