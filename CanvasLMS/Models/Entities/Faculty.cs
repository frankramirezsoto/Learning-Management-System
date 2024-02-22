namespace CanvasLMS.Models.Entities
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        List<Career> Careers { get; set; }
    }
}
