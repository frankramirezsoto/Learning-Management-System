namespace CanvasLMS.Models.Entities
{
    public class ModulePath
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int CourseCycleId { get; set; }
        public string? Name { get; set; }
        public string Path { get; set; }

        public Module? Module { get; set; }
    }
}
