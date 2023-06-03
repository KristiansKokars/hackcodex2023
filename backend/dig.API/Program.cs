using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add database config
// builder.Services.AddDbContext<DigitexDb>(opt => opt.UseInMemoryDatabase("DigitexInMem"));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/process", async () => 
{
    var recognizedInvoice = await AzureAIService.RecognizeInvoiceModel("https://github.com/Azure-Samples/cognitive-services-REST-api-samples/raw/master/curl/form-recognizer/rest-api/invoice.pdf");
});

app.MapPost("/upload", async (IFormFile file) =>
{
    if (file != null && file.Length > 0)
    {
        var fileName = AzureFileService.SaveFileToStorage(file);
        return Results.Ok(fileName);
    }
    else
    {
        return Results.BadRequest();
    }
});

app.Run();
