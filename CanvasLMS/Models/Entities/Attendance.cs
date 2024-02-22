using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(ClassId), nameof(CourseCycleId), nameof(StudentId))]
    public class Attendance
    {
        //Primary Composite Key of 3
        public int ClassId { get; set; }
        public int CourseCycleId { get; set; }
        public int StudentId { get; set; }

        public string Status { get; set; }

        public Class Class { get; set; }
        public Student Student { get; set; }
    }
}
