using QuizBuilder.Clients.Models;
using Refit;

namespace QuizBuilder.Clients
{
    public interface IUserClient
    {

        [Post("/realms/platformservices/protocol/openid-connect/token")]
        public Task<TokenResponse> GetKeycloakAccessToken([Body(BodySerializationMethod.UrlEncoded)] TokenRequest request);

        [Get("/admin/realms/platformservices/keys")]
        public Task<string> GetJwtSigningKey([Header("Authorization")] string auth_token);

        [Post("/admin/realms/platformservices/users")]
        public Task CreateUser([Header("Authorization")] string auth_token, [Body] CreateUserRequest request);

        [Delete("/admin/realms/platformservices/users/{userId}")]
        public Task DeleteUser([Header("Authorization")] string auth_token, string userId);

        [Get("/admin/realms/platformservices/users")]
        public Task<List<GetUserResponse>> GetUser([Header("Authorization")] string auth_token, [Query] string email, [Query] bool exact);

        [Post("/realms/platformservices/protocol/openid-connect/token")]
        public Task<LoginResponse> Login([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request);

    }
}
