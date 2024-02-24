using CanvasLMS.Models.Entities;

namespace CanvasLMS.Models.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string IdFullName => $"{Id.ToString().Substring(Math.Max(0, Id.ToString().Length - 3))} - {FirstName} {LastName}";

        public List<Attendance>? Attendance { get; set; }
        public List<Enrollment>? Enrollments { get; set; }
        public List<Score>? Scores { get; set; }
        public List<Group>? Groups { get; set; }
        public List<TaskSubmission>? Submissions { get; set; }
        public List<Career>? Careers { get; } = [];
    }
}
