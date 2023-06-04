using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace dig.API.Feature.Auth;

public static class AuthRoutes
{
    public static void AddAuth(this WebApplication webApp)
    {
        webApp.UseAuthentication();
        webApp.UseAuthorization();

        webApp.MapPost("/register", Register);
        webApp.MapPost("/login", Login);
    }

    public static void EnableCookieAuthAndAddDependencies(this WebApplicationBuilder builder)
    {
        // TODO: we would have massively preferred simple cookie session based auth here but there were many unexpected problems with cookie setting from C#
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER")!,
                    ValidateIssuer = true,
                    ValidAudience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE")!,
                    ValidateAudience = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("AUTH_KEY")!)),
                    ValidateIssuerSigningKey = true
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<AuthService>();
    }

    // TODO: I dislike passing these through not the header, but currently it is the simplest way
    // TODO: no email verification too for Hackathon version
    private static async Task<IResult> Register([FromHeader] string authorization, HttpContext context,
        AuthService authService)
    {
        var (username, password) = GetCredentialsFromBasicAuth(authorization);
        var authResult = await authService.Register(username, password);
        return authResult.Map<IResult>(
            error: error => Results.BadRequest(error),
            success: user => Results.Ok(CreateTokenString(user)));
    }

    private static async Task<IResult> Login([FromHeader] string authorization, HttpContext context,
        AuthService authService)
    {
        var (username, password) = GetCredentialsFromBasicAuth(authorization);
        var authResult = await authService.Login(username, password);
        return authResult.Map<IResult>(
            error: error => Results.BadRequest(error),
            success: user => Results.Ok(CreateTokenString(user)));
    }

    // TODO: no error checking here, we shall trust the user for now, otherwise implement Either construct here in future
    private static (string, string) GetCredentialsFromBasicAuth(string basicAuthHeader)
    {
        var encoding = Encoding.GetEncoding("iso-8859-1");
        var credentialString = encoding.GetString(Convert.FromBase64String(basicAuthHeader.Split(" ")[1]));
        var credentials = credentialString.Split(':');
        return (credentials[0], credentials[1]);
    }

    private static string CreateTokenString(UserDto user)
    {
        var authKey = Environment.GetEnvironmentVariable("AUTH_KEY")!;
        var authIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER")!;
        var audience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE")!;

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey));
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("Username", user.Username),
            new Claim("Id", user.Id)
        };

        var expirationDate = DateTime.UtcNow.AddHours(1);
        var jwtToken = new JwtSecurityToken(
            authIssuer,
            audience,
            expires: expirationDate,
            signingCredentials: credentials,
            claims: claims
        );
        var jwtString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return jwtString;
    }
}