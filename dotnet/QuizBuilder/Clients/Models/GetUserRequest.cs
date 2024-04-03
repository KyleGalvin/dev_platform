namespace QuizBuilder.Clients.Models
{
    public class GetUserRequest
    {
        public bool exact { get; set; } = true;
        public string email { get; set; }
    }
}
