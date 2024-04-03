using System.Text.Json.Serialization;

namespace UserManager.Sdk.Models
{
    public class RoleMappingsResponse
    {
        [JsonPropertyName("realmMappings")]
        public List<RoleMapping> RealmMappings { get; set; } = new List<RoleMapping>();

        [JsonPropertyName("clientMappings")]
        public ClientMappings ClientMappings { get; set; }
    }

    public class ClientMappings() 
    {
        [JsonPropertyName("realm-management")]
        public RealmManagement RealmManagement { get; set; }
    }

    public class RealmManagement 
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("client")]
        public string Client { get; set; }

        [JsonPropertyName("mappings")]
        public List<RoleMapping> Mappings { get; set; } = new List<RoleMapping>();
    }

    public class RoleMapping 
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
