using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace dig.API.Feature.Auth;

public static class AuthRoutes
{
    public static void AddAuth(this WebApplication webApp)
    {
        webApp.UseAuthentication();

        webApp.MapPost("/register", Register);
        webApp.MapPost("/login", Login);
    }

    public static void EnableCookieAuthAndAddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication("cookie")
            .AddCookie("cookie");
        builder.Services.AddScoped<AuthService>();
    }

    // TODO: I dislike passing these through not the header, but currently it is the simplest way
    // TODO: no email verification too for Hackathon version
    private static async Task<IResult> Register([FromHeader] string authorization, HttpContext context, AuthService authService)
    {
        var (username, password) = GetCredentialsFromBasicAuth(authorization);
        var authResult = await authService.Register(username, password);
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
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync("cookie", userPrincipal);
                return Results.Ok(user);
            }
        );
    }

    private static async Task<IResult> Login([FromHeader] string authorization, HttpContext context, AuthService authService)
    {
        var (username, password) = GetCredentialsFromBasicAuth(authorization);
        var authResult = await authService.Login(username, password);
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
                var userPrincipal = new ClaimsPrincipal(claimsIdentity);
                await context.SignInAsync("cookie", userPrincipal);
                return Results.Ok(user);
            }
        );
    }

    private static async Task SignOut(HttpContext context)
    {
        await context.SignOutAsync("cookie");
    }

    // TODO: no error checking here, we shall trust the user for now, otherwise implement Either construct here in future
    private static (string, string) GetCredentialsFromBasicAuth(string basicAuthHeader)
    {
        var encoding = Encoding.GetEncoding("iso-8859-1");
        var credentialString = encoding.GetString(Convert.FromBase64String(basicAuthHeader.Split(" ")[1]));
        var credentials = credentialString.Split(':');
        return (credentials[0], credentials[1]);
    }
}