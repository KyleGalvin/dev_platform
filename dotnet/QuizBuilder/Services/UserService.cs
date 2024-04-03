using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json.Linq;
using QuizBuilder.Clients;
using QuizBuilder.Clients.Models;
using QuizBuilder.Models;
using QuizBuilder.Util;
using System.Security.Cryptography;
using System.Text;

namespace QuizBuilder.Services
{
    public class UserService
    {
        private readonly IUserClient _userClient;
        private readonly string _grantType = "client_credentials";
        private readonly ILogger<UserService> _logger;
        private string _token;
        private string _bearer_token { get => "Bearer " + _token; }

        public UserService(ILogger<UserService> logger, IUserClient userClient) 
        { 
            _logger = logger;
            _userClient = userClient;
        }

        private async Task RefreshKeycloakAccessToken() 
        {
            if (string.IsNullOrEmpty(_token))
            {
                try 
                {
                    var tokenRequest = new TokenRequest()
                    {
                        client_id = EnvironmentVars.GetKeycloakClientId(),
                        client_secret = EnvironmentVars.GetKeycloakApiSecret(),
                        grant_type = _grantType
                    };
                    var response = await _userClient.GetKeycloakAccessToken(tokenRequest);
                    _token = response.access_token;
                } catch(Exception ex) 
                {
                    _logger.LogError("Could not refresh application auth token");
                }
                
            }
        }

        private static byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default:
                    throw new System.Exception(
                  "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        public User GetUserInfo(string token) 
        {
            var parts = token.Split('.');
            var payload = Base64UrlDecode(parts[1]);
            var stringPayload = Encoding.Default.GetString(payload);
            var jsonData = JObject.Parse(stringPayload);
            var subject = (string)jsonData["sub"];
            var email = (string)jsonData["email"];
            return new User() 
            { 
                Id = subject,
                Email = email,
            };
        }

        public async Task<Result<string>> Login(string email, string password) 
        {
            var loginRequest = new LoginRequest()
            {
                client_id = EnvironmentVars.GetKeycloakClientId(),
                client_secret = EnvironmentVars.GetKeycloakApiSecret(),
                username = email,
                password = password,
                grant_type = "password"
            };
            try
            {
                var response = await _userClient.Login(loginRequest);
                return new Result<string>(response.access_token);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"login failed: {ex.Message}");
                return new Result<string>((int)ServiceErrorCodes.LoginFailed, "Login failed");
            }

        }

        public async Task<Result<User>> CreateUser(string email, string password) 
        {
            var createUserRequest = new CreateUserRequest()
            {
                username = email,
                email = email,
                credentials = new List<UserCredentials>() { new UserCredentials()
                {
                    value = password
                } }
            };

            await RefreshKeycloakAccessToken();
            try
            {
                await _userClient.CreateUser(_bearer_token, createUserRequest);
                var users = await _userClient.GetUser(_bearer_token, email, true);
                var singleUser = new User() { Id = users.First().id, Email = users.First().email };
                return new Result<User>(singleUser);
            }
            catch (Exception ex) 
            {
                _logger.LogError("Could not create user");
                return new Result<User>((int)ServiceErrorCodes.CreateUserFailed, "Could not create user");
            }

        }

        public async Task<Result> DeleteUser(string userId)
        {
            await RefreshKeycloakAccessToken();
            try
            {
                await _userClient.DeleteUser(_bearer_token, userId);
                return new Result();
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not delete user");
                return new Result((int)ServiceErrorCodes.EntityNotFound, "Could not delete user");
            }

        }
    }
}
