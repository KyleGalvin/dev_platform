using Microsoft.AspNetCore.Authentication;
using QuizBuilder.Database.Adapters;
using QuizBuilder.Models;
using QuizBuilder.Util;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("XUnitIntegrationTests")]
namespace QuizBuilder.Services
{
    public class QuizResponseService
    {
        private readonly ILogger<QuizResponse> _logger;
        private readonly QuizResponseAdapter _adapter;
        private readonly QuizAdapter _quizAdapter;
        private readonly QuizQuestionAdapter _quizQuestionAdapter;
        private readonly QuizQuestionChoiceAdapter _quizQuestionChoiceAdapter;

        public QuizResponseService(ILogger<QuizResponse> logger, QuizResponseAdapter adapter, QuizAdapter quizAdapter, QuizQuestionAdapter quizQuestionAdapter, QuizQuestionChoiceAdapter quizQuestionChoiceAdapter)
        {
            _logger = logger;
            _adapter = adapter;
            _quizAdapter = quizAdapter;
            _quizQuestionAdapter = quizQuestionAdapter;
            _quizQuestionChoiceAdapter = quizQuestionChoiceAdapter;

        }
        public async Task<Result<QuizResponse>> CreateQuizResponse(QuizResponse quizResponse, string currentUserId)
        {
            var quiz = _quizAdapter.GetQuiz(quizResponse.QuizId);
            if (currentUserId == quiz.OwnerId)
            {
                return new Result<QuizResponse>((int)ServiceErrorCodes.CannotAnswerOwnQuiz, "Cannot answer your own quiz");
            }

            if (!quiz.Published.Value) 
            {
                return new Result<QuizResponse>((int)ServiceErrorCodes.QuizNotPublished, "Cannot answer unpublished quiz");
            }

            var previousAnswer = _adapter.GetQuizResponseDetails(quizResponse.QuizId, currentUserId);
            if (previousAnswer.Any()) 
            {
                return new Result<QuizResponse>((int)ServiceErrorCodes.QuizAlreadyAnswered, "Cannot answer a quiz more than once");
            }
            var quizQuestions = _quizQuestionAdapter.GetQuestionsForQuiz(quizResponse.QuizId);


            //cannot supply multiple answers to a single answer question
            foreach(var q in quizQuestions) 
            {
                var choices = _quizQuestionChoiceAdapter.GetChoicesForQuestion(q.Id);
                if (choices.Where(c => c.IsCorrect).Count() == 1 && quizResponse.QuizResponseDetails.Where(d => d.QuizQuestionId == q.Id).Count() > 1) 
                {
                    return new Result<QuizResponse>((int)ServiceErrorCodes.TooManyQuestionAnswers, "Cannot give multiple answers to a single answer question");
                }
            }

            foreach (var d in quizResponse.QuizResponseDetails) 
            { 
                d.Id = Guid.NewGuid().ToString();
                d.OwnerId = currentUserId;
                await _adapter.CreateQuizResponseDetail(d);
            }
            
            return new Result<QuizResponse>(quizResponse);
        }

        public async Task<Result> DeleteQuizResponse(string quizId, string ownerId, string currentUser) 
        {
            var quiz = _quizAdapter.GetQuiz(quizId);
            if (currentUser != quiz.OwnerId) 
            {
                return new Result((int)ServiceErrorCodes.NotEntityOwner, "Only the quiz owner can delete responses");
            }

            await _adapter.DeleteQuizResponse(quizId, ownerId);
            return new Result();
        }

        public async Task<Result<QuizResponse>> GetQuizResponse(string quizId, string ownerId, User currentUser)
        {
            if (ownerId == currentUser.Id) 
            {
                return new Result<QuizResponse>((int)ServiceErrorCodes.CannotReadYourResponses, "Cannot get your own responses. Look for your scores on the reports endpoint instead");
            }

            var result = new QuizResponse();
            result.QuizId = quizId;
            result.QuizResponseDetails = _adapter.GetQuizResponseDetails(quizId, ownerId);

            return new Result<QuizResponse>(result);
        }

        public async Task<Result<QuizScore>> GetQuizResponseScore(string quizId, string ownerId, User currentUser)
        {
            var quiz = _quizAdapter.GetQuiz(quizId);
            var quizScore = new QuizScore();

            var quizResponseDetails = _adapter.GetQuizResponseDetails(quizId, ownerId);
            if (quiz.OwnerId == currentUser.Id)
            {
                quizScore.QuizResponseDetails = quizResponseDetails;
            }
            quizScore.QuizId = quizId;

            var quizQuestions = _quizQuestionAdapter.GetQuestionsForQuiz(quizId);

            var questionScores = new List<QuestionScore>();
            foreach (var q in quizQuestions) 
            {
                var allChoices = _quizQuestionChoiceAdapter.GetChoicesForQuestion(q.Id);
                questionScores.Add(new QuestionScore() 
                { 
                    QuestionId = q.Id,
                    Score = GetQuestionScore( allChoices, quizResponseDetails.Where(r => r.QuizQuestionId == q.Id))
                });
            }
            quizScore.QuestionScores = questionScores;

            //calculate the total score for the quiz
            float sum = 0;
            int count = 0;
            foreach (var q in quizScore.QuestionScores) 
            {
                sum += q.Score;
                count++;
            }
            quizScore.TotalScore = sum / count;

            return new Result<QuizScore>(quizScore);
        }

        internal static float GetQuestionScore(IEnumerable<QuizQuestionChoice> allChoices, IEnumerable<QuizResponseDetail> selections) 
        {
            
            var possibleWrongAnswers = allChoices.Where(c => !c.IsCorrect).Select(c => c.Id);
            var possibleRightAnswers = allChoices.Where(c => c.IsCorrect).Select(c => c.Id);
            var selectedRightAnswers = selections.Select(s => s.QuizQuestionChoiceId).Intersect(possibleRightAnswers);
            var selectedWrongAnswers = selections.Select(s => s.QuizQuestionChoiceId).Intersect(possibleWrongAnswers);

            var positiveScore = possibleRightAnswers.Count() == 0 ? 0 : (float)selectedRightAnswers.Count() / (float)possibleRightAnswers.Count();
            var negativeScore = possibleWrongAnswers.Count() == 0 ? 0 : (float)selectedWrongAnswers.Count() / (float)possibleWrongAnswers.Count();
            return positiveScore - negativeScore;
        }
    }
}
