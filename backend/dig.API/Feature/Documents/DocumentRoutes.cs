using System.Text.Json;

namespace dig.API.Feature.Documents;

// TODO: ! think about retrieving and passing user id !!! 
public static class DocumentRoutes
{
    public static void AddDocumentDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DocumentService>();
    }
    
    public static void AddDocuments(this WebApplication webApp)
    {
        webApp.MapPost("/upload", Upload);
        webApp.MapPost("/resolve/{id}", Resolve);
        webApp.MapGet("/docs", GetDocuments);
        webApp.MapGet("/docs/faulty", GetFaultyDocuments);
        webApp.MapGet("/docs/{id}", GetDocumentById);
    }

    // piem, jāposto uz localhost:7050/resolve/f5156900-0253-11ee-be56-0242ac120002
    private static async Task<IResult> Resolve(HttpContext context, string id)
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
    }

    // TODO: consider one endpoint for docs + filtering
    private static IResult GetDocuments(HttpContext context, DocumentService documentService)
    {
        // TODO: include user ID
        var docs = documentService.GetDocuments();
        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            Content = JsonSerializer.Deserialize<DocContentDto>(d.Content),
            Link = d.Link,
            Status = d.Status
        }).ToList();

        return Results.Ok(documentDtos);
    }

    private static async Task<IResult> GetFaultyDocuments(HttpContext context, DocumentService documentService)
    {
        // TODO: include user ID
        var docs = documentService.GetFaultyDocuments();
        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            Content = JsonSerializer.Deserialize<DocContentDto>(d.Content),
            Link = d.Link,
            Status = d.Status
        }).ToList();

        return Results.Ok(documentDtos);
    }

    private static async Task<IResult> GetDocumentById(HttpContext context, string id)
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
    }

    private static async Task<IResult> Upload(HttpContext context)
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
                var fieldConfidence = (double)rowContent["Confidence"];

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
    }
}
