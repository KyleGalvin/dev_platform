namespace QuizBuilder.Clients.Models
{
    public class CreateUserRequest
    {
        public string username { get; set; }
        public string email { get; set; }
        public List<UserCredentials> credentials { get; set; }
        public bool enabled { get; set; } = true;
      
    }

    public class UserCredentials 
    {
        public string type { get; set; } = "password";
        public bool temporary { get; set; } = false;
        public string value { get; set; }
    }
}
