using QuizBuilder.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitIntegrationTests.Clients
{
    internal interface IQuizResponseClient
    {
        [Post("/QuizResponse/CreateQuizResponse")]
        public Task<IApiResponse<QuizResponse>> CreateQuizResponse([Header("Authorization")] string bearerToken, [Body] QuizResponse quizResponse);

        [Delete("/QuizResponse/DeleteQuizResponse")]
        public Task<IApiResponse> DeleteQuizResponse([Header("Authorization")] string bearerToken, string quizId, string ownerId);

        [Get("/QuizResponse/GetQuizResponse")]
        public Task<IApiResponse<QuizResponse>> GetQuizResponse([Header("Authorization")] string bearerToken, string quizId, string ownerId);

        [Get("/QuizResponse/GetQuizResponseScore")]
        public Task<IApiResponse<QuizScore>> GetQuizResponseScore([Header("Authorization")] string bearerToken, string quizId, string ownerId);

    }
}
