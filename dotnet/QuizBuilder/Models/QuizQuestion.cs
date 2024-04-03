using System.Linq;

namespace QuizBuilder.Models
{
    public class QuizQuestion
    {
        public string? Id { get; set; }
        public string QuizId { get; set; }
        public string? Title { get; set; }
        public bool? IsMulti { get; set; }
        public bool HasMultipleAnswers => AnswerChoice.Where(c => c.IsCorrect).Count() > 1;
        public List<QuizQuestionChoice> AnswerChoice { get; set; } = new List<QuizQuestionChoice>();
    }
}
