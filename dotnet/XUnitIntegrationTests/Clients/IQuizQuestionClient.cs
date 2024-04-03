using QuizBuilder.Models;
using Refit;

namespace XUnitIntegrationTests.Clients
{
    internal interface IQuizQuestionClient
    {
        [Post("/QuizQuestion/CreateQuizQuestion")]
        public Task<IApiResponse<QuizQuestion>> CreateQuizQuestion([Header("Authorization")] string bearerToken, [Body] QuizQuestion quizQuestion);

        [Put("/QuizQuestion/UpdateQuizQuestion")]
        public Task<IApiResponse<QuizQuestion>> UpdateQuizQuestion([Header("Authorization")] string bearerToken, [Body] QuizQuestion quizQuestion);

        [Delete("/QuizQuestion/DeleteQuizQuestion")]
        public Task<IApiResponse> DeleteQuizQuestion([Header("Authorization")] string bearerToken, string id);

        [Get("/QuizQuestion/GetQuizQuestion")]
        public Task<IApiResponse<QuizQuestion>> GetQuizQuestion([Header("Authorization")] string bearerToken, string id);
    }
}
