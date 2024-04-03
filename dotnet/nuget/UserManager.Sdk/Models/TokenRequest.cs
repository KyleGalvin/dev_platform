namespace UserManager.Sdk.Models
{
    public class TokenRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string grant_type { get; set; }
    }
}
