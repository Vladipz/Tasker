namespace Tasker.API.Contracts.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}