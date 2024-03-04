using CanvasLMS.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.ViewModels
{
    public class EnrollmentViewModel
    {
        public int CourseCycleId { get; set; }
        public int StudentId { get; set; }

        [Precision(3, 2)]
        public decimal? FinalScore { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public Student? Student { get; set; }
    }
}
