using Azure;
using System.ComponentModel.DataAnnotations;

namespace CanvasLMS.Models.Entities
{
    public class Career
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AcademicLevelId { get; set; }
        public int FacultyId { get; set; }

        public List<Course> Courses { get; } = [];
        public AcademicLevel AcademicLevel { get; set; }
        public Faculty Faculty { get; set; }

    }
}
