namespace CanvasLMS.Models.Entities
{
    public class ClassPath
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int CourseCycleId { get; set; }
        public string? Name { get; set; }
        public string Path { get; set; }

        public Class? Class { get; set; }
    }
}
