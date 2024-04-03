namespace QuizBuilder.Models
{
    public class Quiz
    {
        public string? Id { get; set; }
        public string? OwnerId { get; set; }
        public string Title { get; set; }
        public bool? Published { get; set; }
        public List<QuizQuestion>? Questions { get; set; } = new List<QuizQuestion>();
    }
}
