using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using ErrorOr;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Tasker.BLL.Interfaces;
using Tasker.BLL.Models.User;

namespace Tasker.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        // Constructor that injects JwtSettings via IOptions
        public TokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        // Method that generates a JWT token for a given user
        public ErrorOr<TokenModel> GenerateToken(TaskerUserModel userModel)
        {
            if (userModel == null)
            {
                return Error.Validation("User model cannot be null.");
            }

            // Create a list of claims for the token
            var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userModel.Id.ToString()), // User ID
            new Claim(JwtRegisteredClaimNames.Name, userModel.UserName), // Username
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token identifier (JTI)
        };

            // Ensure the JWT secret is set
            if (string.IsNullOrEmpty(_jwtSettings.Secret))
            {
                return Error.Unexpected("JWT secret is not set.");
            }

            // Generate the signing key from the secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            // Create the JWT token with claims, issuer, audience, expiration, and signing credentials
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            // Return the token model with the generated token and its expiration time
            return new TokenModel
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
            };
        }
    }
}