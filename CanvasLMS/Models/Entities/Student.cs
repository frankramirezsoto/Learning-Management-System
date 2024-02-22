using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Models.Entities
{
    [Index(nameof(Email))]
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Attendance> Attendance { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<Score> Scores { get; set; }
        public List<Group> Groups { get; set; }
    }
}
