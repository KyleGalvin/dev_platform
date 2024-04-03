using QuizBuilder.Models;
using Refit;

namespace XUnitIntegrationTests.Clients
{
    internal interface IQuizQuestionChoiceClient
    {
        [Post("/QuizQuestionChoice/CreateQuizQuestionChoice")]
        public Task<IApiResponse<QuizQuestionChoice>> CreateQuizQuestionChoice([Header("Authorization")] string bearerToken, [Body] QuizQuestionChoice quizQuestionChoice);

        [Put("/QuizQuestionChoice/UpdateQuizQuestionChoice")]
        public Task<IApiResponse<QuizQuestionChoice>> UpdateQuizQuestionChoice([Header("Authorization")] string bearerToken, [Body] QuizQuestionChoice quizQuestionChoice);

        [Delete("/QuizQuestionChoice/DeleteQuizQuestionChoice")]
        public Task<IApiResponse> DeleteQuizQuestionChoice([Header("Authorization")] string bearerToken, string id);

        [Get("/QuizQuestionChoice/GetQuizQuestionChoice")]
        public Task<IApiResponse<QuizQuestionChoice>> GetQuizQuestionChoice([Header("Authorization")] string bearerToken, string id);
    }
}
