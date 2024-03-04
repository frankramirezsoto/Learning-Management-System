using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(Id), nameof(CourseCycleId))]
    public class Class
    {
        //Composite Primary Key of 2
        public int Id { get; set; }
        public int CourseCycleId { get; set; }

        public int ProfessorId { get; set; }
        public string ClassTitle { get; set; }
        public DateTime ClassDate { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public Professor? Professor { get; set; }
        public List<Attendance>? Attendance { get; set; }
        public List<ClassPath>? FilePaths { get; set; }
    }
}
