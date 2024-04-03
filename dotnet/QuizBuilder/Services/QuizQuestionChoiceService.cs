using Microsoft.AspNetCore.Http.HttpResults;
using QuizBuilder.Database.Adapters;
using QuizBuilder.Models;
using QuizBuilder.Util;

namespace QuizBuilder.Services
{
    public class QuizQuestionChoiceService
    {
        private readonly ILogger<QuizQuestionChoiceService> _logger;
        private readonly QuizQuestionChoiceAdapter _adapter;
        private readonly QuizQuestionAdapter _quizQuestionAdapter;
        private readonly QuizAdapter _quizAdapter;
        public QuizQuestionChoiceService(ILogger<QuizQuestionChoiceService> logger, QuizQuestionChoiceAdapter adapter, QuizQuestionAdapter quizQuestionAdatper, QuizAdapter quizAdapter)
        {
            _logger = logger;
            _adapter = adapter;
            _quizQuestionAdapter = quizQuestionAdatper;
            _quizAdapter = quizAdapter;

        }

        public async Task<Result<QuizQuestionChoice>> CreateQuizQuestionChoice(QuizQuestionChoice quizQuestionChoice, User currentUser)
        {

            var quizQuestion = _quizQuestionAdapter.GetQuizQuestion(quizQuestionChoice.QuizQuestionId);
            var quiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);

            //validate the current user is allowed to make question choices on this quiz
            if (quiz.OwnerId != currentUser.Id)
            {
                return new Result<QuizQuestionChoice>((int)ServiceErrorCodes.NotEntityOwner, "User not authorized to create question choices on this quiz");
            }

            //validate we dont have 5 choices already
            var existingQuestions = _adapter.GetChoicesForQuestion(quizQuestionChoice.QuizQuestionId);
            if (existingQuestions.Count() >= 5)
            {
                return new Result<QuizQuestionChoice>((int)ServiceErrorCodes.ValueOutOfRange, "Max Choice Count Exceeded");
            }

            quizQuestionChoice.Id = Guid.NewGuid().ToString();
            await _adapter.CreateQuizQuestionChoice(quizQuestionChoice);

            //if there are more than one choices on this question, make it a multi. Else, make it single.
            var otherChoices = _adapter.GetChoicesForQuestion(quizQuestion.Id);
            var questionIsMulti = otherChoices.Count() > 1;

            quizQuestion.IsMulti = questionIsMulti;
            await _quizQuestionAdapter.UpdateQuizQuestion(quizQuestion);

            return new Result<QuizQuestionChoice>(quizQuestionChoice);
        }

        public async Task<Result> DeleteQuizQuestionChoice(string id, User currentUser)
        {
            var quizQuestionChoice = _adapter.GetQuizQuestionChoice(id);
            var quizQuestion = _quizQuestionAdapter.GetQuizQuestion(quizQuestionChoice.QuizQuestionId);
            var quiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);
            if (currentUser.Id != quiz.OwnerId)
            {
                return new Result((int)ServiceErrorCodes.NotEntityOwner, "Cannot delete a question on a quiz you dont own");
            }
            await _adapter.DeleteQuizQuestionChoice(id);

            //if there are more than one choices on this question, make it a multi. Else, make it single.
            var otherChoices = _adapter.GetChoicesForQuestion(quizQuestion.Id);
            var questionIsMulti = otherChoices.Count() > 1;

            quizQuestion.IsMulti = questionIsMulti;
            await _quizQuestionAdapter.UpdateQuizQuestion(quizQuestion);

            return new Result();
        }

        public Result<QuizQuestionChoice> GetQuizQuestionChoice(string id)
        {
            var quizQuestionChoice = _adapter.GetQuizQuestionChoice(id);
            if (quizQuestionChoice == null) 
            {
                return new Result<QuizQuestionChoice>((int)ServiceErrorCodes.EntityNotFound, "Question choice not found");
            }
            return new Result<QuizQuestionChoice>(quizQuestionChoice);
        }

        public async Task<Result<QuizQuestionChoice>> UpdateQuizQuestionChoice(QuizQuestionChoice quizQuestionChoice, User currentUser)
        {
            var quizQuestion = _quizQuestionAdapter.GetQuizQuestion(quizQuestionChoice.QuizQuestionId);
            var quiz = _quizAdapter.GetQuiz(quizQuestion.QuizId);

            if (currentUser.Id != quiz.OwnerId)
            {
                return new Result<QuizQuestionChoice>((int)ServiceErrorCodes.NotEntityOwner, "Cannot update a question choice on a quiz you dont own");
            }
            await _adapter.UpdateQuizQuestionChoice(quizQuestionChoice);

            //if there are more than one choices on this question, make it a multi. Else, make it single.
            var otherChoices = _adapter.GetChoicesForQuestion(quizQuestion.Id);
            var questionIsMulti = otherChoices.Count() > 1;

            quizQuestion.IsMulti = questionIsMulti;
            await _quizQuestionAdapter.UpdateQuizQuestion(quizQuestion);
            return new Result<QuizQuestionChoice>(quizQuestionChoice);
        }
    }
}
