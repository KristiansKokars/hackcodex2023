using Microsoft.EntityFrameworkCore;

using System.Text.Json;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add database config
// builder.Services.AddDbContext<DigitexDb>(opt => opt.UseInMemoryDatabase("DigitexInMem"));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(o =>
    {
        o.AllowAnyOrigin()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

// app.MapGet("/process", async () => 
// {
//     var recognizedInvoice = await AzureAIService.RecognizeInvoiceModel("https://github.com/Azure-Samples/cognitive-services-REST-api-samples/raw/master/curl/form-recognizer/rest-api/invoice.pdf");
// });

app.MapPost("/upload", async (HttpContext context) =>
{
    Console.WriteLine(context.Request.Form.Files);

    var files = context.Request.Form.Files;
    var fileNames = new List<string>();

    foreach (var file in files)
    {
        var fileName = await AzureFileService.SaveFileToStorage(file);

        var req = await AzureAIService.RecognizeInvoiceModel(fileName);

        var jsString = JsonSerializer.Serialize(req);

        Console.WriteLine();
        Console.WriteLine(jsString);
        Console.WriteLine();

        Console.WriteLine("File name: " + fileName);
        fileNames.Add(fileName);
    }

    return Results.Ok(fileNames);
});

app.Run();
