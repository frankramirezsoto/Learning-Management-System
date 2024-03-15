using CanvasLMS.Models.Entities;

namespace CanvasLMS.Models.ViewModels
{
    public class GroupViewModel
    {
        public int? Id { get; set; }
        public int CourseCycleId { get; set; }

        public CourseCycle? CourseCycle { get; set; }
        public List<Student>? Students { get; set; }
        public List<int>? StudentIds { get; set; }
    }
}
