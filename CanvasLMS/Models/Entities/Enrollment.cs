using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(CourseCycleId), nameof(StudentId))]
    public class Enrollment
    {
        //Composite Primary Key of 4
        public int CourseCycleId { get; set; }
        public int StudentId { get; set; }

        public DateTime Date { get; set; }
        public bool? IsCancelled { get; set; }
        public string? CancelReason { get; set; }
        public bool CourseCompleted { get; set; }
        [Precision(3, 2)]
        public decimal FinalScore { get; set; }

        public CourseCycle CourseCycle { get; set; }
        public Student Student { get; set; }
    }
}
