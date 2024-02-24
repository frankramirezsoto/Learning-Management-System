using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Models.Entities
{
    public class CourseCycle
    {
        public int Id { get; set; }

        public string CourseId { get; set; }
        public string CycleId { get; set; }

        public int GroupNo { get; set; }
        public int? ProfessorId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan TimeOfDay { get; set; }
        public int maxQuota { get; set; }
        public string? MeetingUrl { get; set; }
        public string Color { get; set; }

        public Course? Course { get; set; }
        public Cycle? Cycle { get; set; }
        public Professor? Professor { get; set; }
        public List<Class>? Classes { get;}
        public List<Enrollment>? Enrollments { get; set; }
        public List<EvaluationItem>? EvaluationItems { get; set; }
        public List<Group>? Groups { get; set; }
    }
}
