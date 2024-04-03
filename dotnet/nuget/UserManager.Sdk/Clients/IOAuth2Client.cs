using Refit;
using UserManager.Sdk.Models;

namespace UserManager.Sdk.Clients
{
    public interface IOAuth2Client
    {
        [Post("/realms/{realm}/protocol/openid-connect/token")]
        public Task<TokenResponse> GetKeycloakAccessToken(string realm, [Body(BodySerializationMethod.UrlEncoded)] TokenRequest request);

        [Get("/admin/{realm}/console/whoami")]
        public Task<ApiResponse<string>> GetRealm([Header("Authorization")] string auth_token, string realm);

        [Post("/admin/realms")]
        public Task<ApiResponse<string>> CreateRealm([Header("Authorization")] string auth_token, RealmRequest request);

        [Get("/admin/realms/{realm}/clients")]
        public Task<IEnumerable<ClientResponse>> GetRealmClients([Header("Authorization")] string auth_token, string realm, [Query] string clientId, [Query] bool search = true);

        [Post("/admin/realms/{realm}/clients")]
        public Task<ApiResponse<string>> CreateRealmClient([Header("Authorization")] string auth_token, string realm, ClientRequest request);

        [Get("/admin/realms/{realm}/users/{id}/role-mappings")]
        public Task<ApiResponse<RoleMappingsResponse>> GetRoleMappings([Header("Authorization")] string auth_token, string realm, string id);
        [Get("/admin/realms/{realm}/users/{id}/role-mappings")]
        public Task<ApiResponse<string>> GetRoleMappingsAlt([Header("Authorization")] string auth_token, string realm, string id);

        [Get("/admin/realms/{realm}/clients/{id}/service-account-user")]
        public Task<ClientServiceAccount> GetClientServiceAccount([Header("Authorization")] string auth_token, string realm, string id);

        [Get("/admin/realms/{realm}/ui-ext/available-roles/users/{id}")]
        public Task<IEnumerable<ClientRole>> GetRole([Header("Authorization")] string auth_token, string realm, string id, [Query] string search, [Query] int first = 0, [Query] int max = 11);

        [Post("/admin/realms/{realm}/users/{id}/role-mappings/clients/{clientId}")]
        public Task<ApiResponse<RoleMappingsResponse>> MapRoleToClient([Header("Authorization")] string auth_token, string realm, string id, string clientId, IEnumerable<RoleMapping> request);
    }
}
