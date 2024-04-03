namespace QuizBuilder.Models
{
    public class QuizQuestionChoice
    {
        public string? Id { get; set; }
        public string QuizQuestionId { get; set; }
        public string? Title { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}
