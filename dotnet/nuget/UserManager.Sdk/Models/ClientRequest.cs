namespace UserManager.Sdk.Models
{
    public class ClientRequest
    {
        public string clientId { get; set; }
        public string secret { get; set; }
        public bool enabled { get; set; } = true;
        public bool standardFlowEnabled { get; set; } = true;
        public bool directAccessGrantsEnabled { get; set; } = true;
        public bool publicClient { get; set; } = true;
        public bool serviceAccountsEnabled { get; set; } = false;
        public List<string> redirectUris {get;set;} = new List<string>();

    }
}
