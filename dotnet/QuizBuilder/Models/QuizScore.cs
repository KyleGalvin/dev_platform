namespace QuizBuilder.Models
{
    public class QuizScore : QuizResponse
    {
        public List<QuestionScore> QuestionScores { get; set; } = new List<QuestionScore>();
        public float TotalScore { get; set; }
    }

    public class QuestionScore 
    { 
        public string QuestionId { get; set; }
        public float Score { get; set; }
    }
}
