using QuizBuilder.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XUnitIntegrationTests.Clients
{
    internal interface IUserClient
    {
        [Post("/User/Login")]
        public Task<IApiResponse<string>> Login(string email, string password);

        [Post("/User/CreateUser")]
        public Task<IApiResponse<User>> CreateUser(string email, string password);

        [Delete("/User/DeleteUser")]
        public Task<IApiResponse> DeleteUser([Header("Authorization")] string bearerToken, string userId);
    }
}
