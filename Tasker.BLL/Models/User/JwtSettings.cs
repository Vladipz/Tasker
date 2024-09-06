namespace Tasker.BLL.Models.User
{
    public class JwtSettings
    {
        public string Secret { get; set; }

        public string ValidIssuer { get; set; }

        public string ValidAudience { get; set; }

        public int TokenExpirationMinutes { get; set; } // Add this property
    }
}