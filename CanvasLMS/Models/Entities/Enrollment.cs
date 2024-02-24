using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(CourseCycleId), nameof(StudentId))]
    public class Enrollment
    {
        //Composite Primary Key of 2
        public int CourseCycleId { get; set; }
        public int StudentId { get; set; }

        [Precision(10, 2)]
        public decimal? FinalScore { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public Student? Student { get; set; }
    }
}
