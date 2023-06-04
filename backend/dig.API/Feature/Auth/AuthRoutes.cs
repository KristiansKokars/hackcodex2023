using System.Security.Claims;
using Azure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace dig.API.Feature.Auth;

public static class AuthRoutes
{
    public static void AddAuth(this WebApplication webApp)
    {
        webApp.UseAuthentication();

        webApp.MapPost("/register", Register);
        webApp.MapPost("/login", Login);
    }

    public static void EnableCookieAuth(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication("cookie")
            .AddCookie("cookie");
    }

    private static async Task<IResult> Register(string username, string email, string password, HttpContext context, AuthService authService)
    {
        var authResult = await authService.Register(username, email, password);
        return await authResult.MapAsync<IResult>(
            error: async error => Results.BadRequest(error),
            success: async user =>
            {
                var claims = new List<Claim>
                {
                    new("id", user.Id),
                    new("username", user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userDto = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync("cookie", userDto);
                return Results.Ok(userDto);
            }
        );
    }

    private static async Task<IResult> Login(string email, string password, HttpContext context, AuthService authService)
    {
        var authResult = await authService.Login(email, password);
        return await authResult.MapAsync<IResult>(
            error: async error => Results.BadRequest(error),
            success: async user =>
            {
                var claims = new List<Claim>
                {
                    new("id", user.Id),
                    new("username", user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var userDto = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync("cookie", userDto);
                return Results.Ok(userDto);
            }
        );
    }

    private static async Task SignOut(HttpContext context)
    {
        await context.SignOutAsync("cookie");
    }
}