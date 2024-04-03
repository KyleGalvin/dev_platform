namespace QuizBuilder.Models
{
    public class QuizResponse
    {
        public string QuizId { get; set; }
        public IEnumerable<QuizResponseDetail> QuizResponseDetails { get; set; } = new List<QuizResponseDetail>();

    }

    public class QuizResponseDetail 
    {
        public string? Id { get; set; }
        public string QuizId { get; set; }
        public string QuizQuestionId { get; set; }
        public string QuizQuestionChoiceId { get; set; }
        public string? OwnerId { get; set; }
    }
}
