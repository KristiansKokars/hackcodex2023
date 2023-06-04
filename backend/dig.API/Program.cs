using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using dig.API.Feature.Auth;
using dig.API.Feature.Documents;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add database config
var connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
builder.Services.AddDbContext<DigitexDb>(options => options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(o =>
    {
        o.AllowAnyOrigin()
            .AllowAnyHeader();
    });
});

builder.AddDocumentDependencies();
builder.EnableCookieAuthAndAddDependencies();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HackCodex 2023 DigiTex API", Version = "v1" });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DigiTex API V1");
        c.RoutePrefix = string.Empty;
        c.DocExpansion(DocExpansion.List);
    });
}

app.MapGet("/", () => "Health Check!");
app.AddAuth();
app.AddDocuments();

app.Run();
