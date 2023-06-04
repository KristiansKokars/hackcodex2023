using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add database config
var connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
builder.Services.AddDbContext<DigitexDb>(options =>
    options.UseSqlServer(connection));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(o =>
    {
        o.AllowAnyOrigin()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<DocumentService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HackCodex 2023 DigiTex API", Version = "v1" });
});

var app = builder.Build();

app.UseRouting();
app.UseCors();
// Configure the HTTP request pipeline.
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

// ! think about retrieving and passing user id !!! 
app.MapGet("/", () => "Hello World!");

// TODO: add error handling and proper response messages
app.MapPost("/upload", async (HttpContext context) =>
{
    Console.WriteLine(context.Request.Form.Files);

    var files = context.Request.Form.Files;
    var fileNames = new List<string>();
    
    // TODO: receive from request (or store in config?)
    double minRequiredPrecison = 0.85;
    Guid userId = Guid.NewGuid();

    var docService = context.RequestServices.GetRequiredService<DocumentService>();
    var filesToInsert = new List<Document>();

    foreach (var file in files)
    {
        // TODO: error handling here
        var fileName = await AzureFileService.SaveFileToStorage(file);
        Console.WriteLine("File name: " + fileName);

        // TODO: error handling here
        var req = await AzureAIService.RecognizeInvoiceModel(fileName);
        var docStatus = "Correct";

        foreach (var documentRow in req)
        {
            var name = documentRow.Key;
            var rowContent = documentRow.Value;

            var fieldValue = rowContent["Value"];
            var fieldConfidence = (double) rowContent["Confidence"];

            if (fieldConfidence < minRequiredPrecison) 
            {
                docStatus = "Faulty";
            }

            Console.WriteLine($"Row: {name}, Value: {fieldValue}, %: {fieldConfidence}");
        }

        var content = JsonSerializer.Serialize(req);
        var doc = docService.DocumentHelper(content, userId, fileName, docStatus);
        
        filesToInsert.Add(doc);
        fileNames.Add(fileName);
    }

    // TODO: error handling here
    var ids = docService.SaveDocumentBatch(filesToInsert);
    // TODO: determine response content
    return Results.Ok(fileNames);
});

// piem, jāposto uz localhost:7050/resolve/f5156900-0253-11ee-be56-0242ac120002
app.MapPost("/resolve/{id}", async (HttpContext context, string id) =>
{
    Console.WriteLine($"Document id: {id}");
    var documentId = Guid.Parse(id);
    var docService = context.RequestServices.GetRequiredService<DocumentService>();

    // Read the request body as a string
    using var reader = new StreamReader(context.Request.Body);
    var requestBody = await reader.ReadToEndAsync();

    try
    {
        var docContents = JsonSerializer.Deserialize<DocContentDto>(requestBody);
        
        // TODO: evaluate if format correct
        
        var resolved = docService.MarkDocumentSolved(documentId, JsonSerializer.Serialize(docContents));
        Console.WriteLine($"Resolved doc id: {resolved}");
        return Results.Ok(resolved);
    }
    catch (JsonException e)
    {
        Console.WriteLine(e.Message);
        return Results.BadRequest();
    }
});

// TODO: consider one endpoint for docs + filtering
app.MapGet("/docs", async (HttpContext context) =>
{
    var docService = context.RequestServices.GetRequiredService<DocumentService>();

    // TODO: include user ID
    var docs = docService.GetDocuments();
    var documentDtos = docs.Select(d => new DocumentDto
    {
        Id = d.Id,
        Content = JsonSerializer.Deserialize<DocContentDto>(d.Content),
        Link = d.Link,
        Status = d.Status
    }).ToList();

    return Results.Ok(documentDtos);
});

app.MapGet("/docs/faulty", async (HttpContext context) =>
{
    var docService = context.RequestServices.GetRequiredService<DocumentService>();

    // TODO: include user ID
    var docs = docService.GetFaultyDocuments();
    var documentDtos = docs.Select(d => new DocumentDto
    {
        Id = d.Id,
        Content = JsonSerializer.Deserialize<DocContentDto>(d.Content),
        Link = d.Link,
        Status = d.Status
    }).ToList();

    return Results.Ok(documentDtos);
});

// TODO: when document saved in blob storage and proper files generated, retrieve label position file
app.MapGet("/docs/{id}", async (HttpContext context, string id) =>
{
    Console.WriteLine($"Document id: {id}");
    var documentId = Guid.Parse(id);
    var docService = context.RequestServices.GetRequiredService<DocumentService>();

    // TODO: include user ID
    var document = docService.GetDocumentById(documentId);
    var documentDto = new DocumentDto
    {
        Id = document.Id,
        Content = JsonSerializer.Deserialize<DocContentDto>(document.Content),
        Link = document.Link,
        Status = document.Status
    };

    return Results.Ok(documentDto);
});

app.Run();
