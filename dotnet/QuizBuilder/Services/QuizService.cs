using QuizBuilder.Controllers;
using QuizBuilder.Database.Adapters;
using QuizBuilder.Models;
using QuizBuilder.Util;

namespace QuizBuilder.Services
{
    public class QuizService
    {
        private readonly ILogger<QuizService> _logger;
        private readonly QuizAdapter _adapter;
        private readonly QuizQuestionAdapter _questionAdapter;
        private readonly QuizQuestionChoiceAdapter _questionChoiceAdapter;
        private readonly QuizResponseAdapter _quizResponseAdapter;
        public QuizService(ILogger<QuizService> logger, QuizAdapter adapter, QuizQuestionAdapter questionAdapter, QuizQuestionChoiceAdapter questionChoiceAdaper, QuizResponseAdapter quizResponseAdapter) 
        {
            _logger = logger;
            _adapter = adapter;
            _questionAdapter = questionAdapter;
            _questionChoiceAdapter = questionChoiceAdaper;
            _quizResponseAdapter = quizResponseAdapter;
        }

        public async Task<Result<Quiz>> CreateQuiz(Quiz quiz, string currentUserId) 
        {
            
            quiz.Id = Guid.NewGuid().ToString();
            quiz.OwnerId = currentUserId;
            quiz.Published = false;//cannot create quiz in a published state
            await _adapter.CreateQuiz(quiz);
            return new Result<Quiz>(quiz);
        }

        public Result<Quiz> GetQuiz(string id)
        {
            var quiz = _adapter.GetQuiz(id);
            return new Result<Quiz>(quiz);
        }

        public Result<IEnumerable<Quiz>> GetQuizzes(int top, int skip)
        {
            var quizzes = _adapter.GetQuizzes(top, skip);
            return new Result<IEnumerable<Quiz>>(quizzes);
        }

        public Result<IEnumerable<Quiz>> GetMyAnsweredQuizzes(int top, int skip, User currentUser) 
        {
            var results = new List<Quiz>();
            var myAnsweredQuizIds = _quizResponseAdapter.GetMyAnsweredQuizzes(top, skip, currentUser.Id);
            foreach (var quizId in myAnsweredQuizIds) 
            {
                var quiz = GetQuiz(quizId);
                if (!quiz.Success) 
                {
                    var Errors = quiz.Errors;
                    Errors.Add(new ErrorResult((int)ServiceErrorCodes.FailedToFetchEntity, "Could not get quiz list"));
                    return new Result<IEnumerable<Quiz>>(Errors);
                }
                results.Add(quiz.Value);
            }
            return new Result<IEnumerable<Quiz>>(results);
        }

        public async Task<Result> DeleteQuiz(string id, User currentUser)
        {
            var existingQuiz = _adapter.GetQuiz(id);
            if (currentUser.Id != existingQuiz.OwnerId)
            {
                return new Result<string>((int)ServiceErrorCodes.NotEntityOwner, "Cannot delete a quiz you dont own");
            }
            await _adapter.DeleteQuiz(id);
            return new Result() { };
        }

        public async Task<Result<Quiz>> UpdateQuiz(Quiz quiz, User currentUser)
        {
            var existingQuiz = _adapter.GetQuiz(quiz.Id);

            if (currentUser.Id != existingQuiz.OwnerId) 
            {
                return new Result<Quiz>((int)ServiceErrorCodes.NotEntityOwner, "Cannot update a quiz you dont own");
            }

            if (existingQuiz.Published.Value) 
            {
                //we cannot update a published quiz
                return new Result<Quiz>((int)ServiceErrorCodes.CannotUpdatePublishedQuiz, "Cannot update a published quiz");
            }

            //if you want to publish, use the publish method, not this one.
            //this is to ensure the validation gets run on publish
            quiz.Published = false;
            quiz.OwnerId = existingQuiz.OwnerId;

            await _adapter.UpdateQuiz(quiz);
            return new Result<Quiz>(quiz);
        }

        public async Task<Result<Quiz>> PublishQuiz(string id)
        {

            var quiz = _adapter.GetQuiz(id);

            //do validation before updating the publish field to true
            var questions = _questionAdapter.GetQuestionsForQuiz(id);
            if (questions.Count() == 0) 
            {
                return new Result<Quiz>((int)ServiceErrorCodes.ValueOutOfRange, "Cannot publish quiz without questions");
            }

            if (questions.Count() > 10)
            {
                return new Result<Quiz>((int)ServiceErrorCodes.ValueOutOfRange, "Cannot publish quiz with more than ten questions");
            }
            foreach (var q in questions) 
            {
                var choices = _questionChoiceAdapter.GetChoicesForQuestion(q.Id);
                if (choices.Count() == 0) 
                {
                    return new Result<Quiz>((int)ServiceErrorCodes.ValueOutOfRange, "Cannot publish quiz question without choices");
                }

                if (choices.Count() > 5)
                {
                    return new Result<Quiz>((int)ServiceErrorCodes.ValueOutOfRange, "Cannot publish quiz question with more than five choices");
                }
            }

            quiz.Published = true;

            await _adapter.UpdateQuiz(quiz);
            return new Result<Quiz>(quiz);
        }
    }
}
