using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using ErrorOr;

using Microsoft.IdentityModel.Tokens;

using Tasker.API.Contracts.Requests;
using Tasker.API.Contracts.Responses;
using Tasker.API.Mapping;
using Tasker.BLL.Interfaces;

namespace Tasker.API.Endpoints
{
    public static class AuthenticationEndpoint
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth");

            group.MapPost("/register", async (RegistrationRequest request, IUserService userService) =>
            {
                var result = await userService.RegisterAsync(request.ToCreateModel());
                return result.Match(
                    user => Results.Created(),
                    error => error.ToResponse());
            });

            group.MapPost("/login", async (
                LoginRequest request,
                IUserService userService,
                ITokenService tokenService) =>
            {
                var result = await userService.ReadAsync(request.Username)
                    .ThenAsync(user => userService.CheckPasswordAsync(user, request.Password))
                    .Then(user => tokenService.GenerateToken(user));

                return result.Match(
                    token => Results.Ok(token.ToResponse()),
                    error => error.ToResponse());
            });
        }
    }
}