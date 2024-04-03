using Microsoft.AspNetCore.Authentication;
using QuizBuilder.Database.Adapters;
using QuizBuilder.Models;
using QuizBuilder.Util;

namespace QuizBuilder.Services
{
    public class QuizQuestionService
    {
        private readonly ILogger<QuizQuestionService> _logger;
        private readonly QuizQuestionAdapter _adapter;
        private readonly QuizAdapter _quizAdapter;
        public QuizQuestionService(ILogger<QuizQuestionService> logger, QuizQuestionAdapter adapter, QuizAdapter quizAdapter)
        {
            _logger = logger;
            _adapter = adapter;
            _quizAdapter = quizAdapter;
        }

        public async Task<Result<QuizQuestion>> CreateQuizQuestion(QuizQuestion quizQuestion, User currentUser)
        {
            var quiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);

            //validate the current user is allowed to make questions on this quiz
            if (quiz.OwnerId != currentUser.Id) 
            {
                return new Result<QuizQuestion>((int)ServiceErrorCodes.NotEntityOwner, "User not authorized to create questions on this quiz");
            }

            //validate we dont have 10 questions already
            var existingQuestions = _adapter.GetQuestionsForQuiz(quizQuestion.QuizId);
            if (existingQuestions.Count() >= 10)
            {
                return new Result<QuizQuestion>((int)ServiceErrorCodes.ValueOutOfRange, "Max Question Count Exceeded");
            }

            quizQuestion.Id = Guid.NewGuid().ToString();
            quizQuestion.IsMulti = false;
            await _adapter.CreateQuizQuestion(quizQuestion);
            return new Result<QuizQuestion>(quizQuestion);
        }

        public async Task<Result> DeleteQuizQuestion(string id, User currentUser)
        {
            var quizQuestion = _adapter.GetQuizQuestion(id);
            var existingQuiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);
            if (currentUser.Id != existingQuiz.OwnerId)
            {
                return new Result((int)ServiceErrorCodes.NotEntityOwner, "Cannot delete a question on a quiz you dont own");
            }
            await _adapter.DeleteQuizQuestion(id);
            return new Result();
        }

        public Result<QuizQuestion> GetQuizQuestion(string id)
        {
            var quizQuestion = _adapter.GetQuizQuestion(id);
            if (quizQuestion == null)
            {
                return new Result<QuizQuestion>((int)ServiceErrorCodes.EntityNotFound, "Question not found");
            }
            return new Result<QuizQuestion>(quizQuestion);
        }

        public async Task<Result<QuizQuestion>> UpdateQuizQuestion(QuizQuestion quizQuestion, User currentUser)
        {
            var existingQuiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);
            if (currentUser.Id != existingQuiz.OwnerId)
            {
                return new Result<QuizQuestion>((int)ServiceErrorCodes.NotEntityOwner, "Cannot update a question on a quiz you dont own");
            }

            await _adapter.UpdateQuizQuestion(quizQuestion);
            return new Result<QuizQuestion>(quizQuestion);
        }
    }
}
