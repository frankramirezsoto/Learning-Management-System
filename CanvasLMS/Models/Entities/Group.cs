using Microsoft.EntityFrameworkCore;

namespace CanvasLMS.Models.Entities
{
    [PrimaryKey(nameof(Id), nameof(CourseCycleId))]
    public class Group
    {
        public int Id { get; set; }
        public int CourseCycleId { get; set; }

        public CourseCycle CourseCycle { get; set; }
        public List<Student> Students { get; set; }
    }
}
