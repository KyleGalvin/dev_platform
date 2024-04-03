using QuizBuilder.Models;
using Refit;

namespace XUnitIntegrationTests.Clients
{
    internal interface IQuizClient
    {
        [Post("/Quiz/CreateQuiz")]
        public Task<IApiResponse<Quiz>> CreateQuiz([Header("Authorization")] string bearerToken, [Body] Quiz quiz);

        [Put("/Quiz/UpdateQuiz")]
        public Task<IApiResponse<Quiz>> UpdateQuiz([Header("Authorization")] string bearerToken, [Body] Quiz quiz);

        [Put("/Quiz/PublishQuiz")]
        public Task<IApiResponse<Quiz>> PublishQuiz([Header("Authorization")] string bearerToken, string id);

        [Delete("/Quiz/DeleteQuiz")]
        public Task<IApiResponse> DeleteQuiz([Header("Authorization")] string bearerToken, string id);

        [Get("/Quiz/GetQuiz")]
        public Task<IApiResponse<Quiz>> GetQuiz([Header("Authorization")] string bearerToken, string id);

        [Get("/Quiz/GetQuizzes")]
        public Task<IApiResponse<IEnumerable<Quiz>>> GetQuizzes([Header("Authorization")] string bearerToken, int top, int skip);

        [Get("/Quiz/GetMyAnsweredQuizzes")]
        public Task<IApiResponse<IEnumerable<Quiz>>> GetMyAnsweredQuizzes([Header("Authorization")] string bearerToken, int top, int skip);
    }
}
