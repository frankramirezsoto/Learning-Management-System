namespace CanvasLMS.Models.Entities
{
    public class EvaluationTask
    {
        public int Id { get; set; }
        public int EvaluationItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public EvaluationItem? EvaluationItem { get; set; }
        public List<TaskSubmission>? Submissions { get; set; }
    }
}
