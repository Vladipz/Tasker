using System.IdentityModel.Tokens.Jwt;

namespace Tasker.API.Helpers
{
    public static class Extensions
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            // I dont check for null here because the user is required to be authenticated
            return Guid.Parse(userId);
        }
    }
}