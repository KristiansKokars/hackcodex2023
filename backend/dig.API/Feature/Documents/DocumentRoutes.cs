using System.Text.Json;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        webApp.MapGet("/docs", GetDocuments).Produces<IEnumerable<DocumentDto>>();
        webApp.MapGet("/docs/faulty", GetFaultyDocuments).Produces<IEnumerable<DocumentDto>>();
        webApp.MapGet("/docs/{id}", GetDocumentById).Produces<DocumentDto>();
    }

    // piem, jāposto uz localhost:7050/resolve/f5156900-0253-11ee-be56-0242ac120002
    [Authorize]
    private static async Task<IResult> Resolve(
        HttpContext context, 
        string id,
        ClaimsPrincipal user)
    {
        Console.WriteLine($"Document id: {id}");
        var documentId = Guid.Parse(id);
        var docService = context.RequestServices.GetRequiredService<DocumentService>();

        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        // Read the request body
        using var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();

        try
        {
            var docContents = JsonSerializer.Deserialize<DocContentDto>(requestBody);

            // TODO: evaluate if format correct

            var resolved = docService.MarkDocumentSolved(
                Guid.Parse(userId), 
                documentId, 
                JsonSerializer.Serialize(docContents));

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
    [Authorize]
    private static IResult GetDocuments(
        HttpContext context, 
        DocumentService documentService,
        ClaimsPrincipal user)
    {
        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        // Retrieve documents
        var docs = documentService.GetDocuments(Guid.Parse(userId));
        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            Content = JsonSerializer.Deserialize<DocContentDto>(d.Content)!,
            Link = d.Link,
            Status = d.Status
        }).ToList();

        return Results.Ok(documentDtos);
    }

    [Authorize]
    private static IResult GetFaultyDocuments(
        HttpContext context, 
        DocumentService documentService,
        ClaimsPrincipal user)
    {
        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        // Retrieve faulty documents
        var docs = documentService.GetFaultyDocuments(Guid.Parse(userId));
        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            Content = JsonSerializer.Deserialize<DocContentDto>(d.Content)!,
            Link = d.Link,
            Status = d.Status
        }).ToList();

        return Results.Ok(documentDtos);
    }

    [Authorize]
    private static async Task<IResult> GetDocumentById(
        HttpContext context, 
        string id, 
        DocumentService documentService,
        ClaimsPrincipal user)
    {
        Console.WriteLine($"Document id: {id}");
        var documentId = Guid.Parse(id);

        // TODO: receive precision from request (or store in config?)
        double minRequiredPrecison = 0.85;
        
        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        var document = documentService.GetDocumentById(Guid.Parse(userId), documentId);
        if (document is null)
        {
            return Results.NotFound($"Document with id: '{userId}' not found!");
        }

        if (document.Status == "Correct")
        {
            var documentDto = new DocumentDto
            {
                Id = document.Id,
                Content = JsonSerializer.Deserialize<DocContentDto>(document.Content)!,
                Link = document.Link,
                Status = document.Status
            };

            return Results.Ok(documentDto);
        }
        else
        {
            var labelsLink = document.Link + ".labels.json";

            var labelsJSONResult = await AzureFileService.RetrieveJSONFromStorage(labelsLink);
            return labelsJSONResult.Map<IResult>(
                error: error => 
                {
                    // TODO: in production, find way to return 500 Internal server error
                    return Results.BadRequest(error);
                },
                success: labelsJSON =>
                {
                    var labelDocument = JsonSerializer.Deserialize<DocLabelsDto>(labelsJSON);
                    var documentContent = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(document.Content);

                    var documentContentMarked = new Dictionary<string, Dictionary<string, object>>();
                    foreach (var documentRow in documentContent!)
                    {
                        var name = documentRow.Key;
                        var rowContent = documentRow.Value;

                        var fieldValue = rowContent["Value"];
                        var fieldConfidence = (double)rowContent["Confidence"];

                        if (fieldConfidence < minRequiredPrecison)
                        {
                            var documentFieldValue = new Dictionary<string, object>();
                            documentFieldValue.Add("Value", fieldValue);
                            documentFieldValue.Add("Confidence", fieldConfidence);
                    
                            var fieldLabels = labelDocument!.Labels
                                .SelectMany(label => label.Value)
                                .SelectMany(labelList => labelList.BoundingBoxes)
                                .Select(box => box)
                                .ToList();
                    
                            documentFieldValue.Add("Labels", fieldLabels);

                            documentContentMarked.Add(name, documentFieldValue);
                        }
                        else
                        {
                            documentContentMarked.Add(documentRow.Key, documentRow.Value);
                        }
                        Console.WriteLine($"Row: {name}, Value: {fieldValue}, %: {fieldConfidence}");
                    }

                    var contentMarkedJSON = JsonSerializer.Serialize(documentContentMarked);

                    var documentDtoMarked = new DocumentDto
                    {
                        Id = document.Id,
                        Content = JsonSerializer.Deserialize<DocContentDto>(contentMarkedJSON)!,
                        Link = document.Link,
                        Status = document.Status
                    };

                    return Results.Ok(documentDtoMarked);
                }
            );
        }
    }

    [Authorize]
    private static async Task<IResult> Upload(
        HttpContext context, 
        DocumentService documentService,
        ClaimsPrincipal user)
    {
        Console.WriteLine(context.User);

        var files = context.Request.Form.Files;
        var fileNames = new List<string>();

        // TODO: receive from request (or store in config?)
        double minRequiredPrecison = 0.85;
        
        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        var filesToInsert = new List<Document>();

        foreach (var file in files)
        {
            var fileNameResult = await AzureFileService.SaveFileToStorage(file);

            // TODO: refactoring
            var fileNameError = false;
            var fileName = fileNameResult.Map<string>(
                error: error => 
                {
                    fileNameError = true;
                    return error.message;
                },
                success: value => {
                    return value;
                }
            );

            if (fileNameError)
            {
                // TODO: in production, find way to return 500 Internal server error
                return Results.BadRequest(fileName);
            }

            Console.WriteLine("File name: " + fileName);

            // TODO: refactoring
            var recognitionResult = await AzureAIService.RecognizeInvoiceModel(fileName);

            var recognitionError = false;
            var reqMaybe = fileNameResult.Map<object>(
                error: error => 
                {
                    recognitionError = true;
                    return error;
                },
                success: value => {
                    return value;
                }
            );

            if (recognitionError)
            {
                // TODO: in production, find way to return 500 Internal server error
                return Results.BadRequest((Common.SimpleMessageError) reqMaybe);
            }

            var req = (Dictionary<string, Dictionary<string, object>>) reqMaybe;
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
            var doc = documentService.DocumentHelper(content, Guid.Parse(userId), fileName, docStatus);

            filesToInsert.Add(doc);
            fileNames.Add(fileName);
        }

        // TODO: refactoring
        try
        {
            var ids = documentService.SaveDocumentBatch(filesToInsert);
            return Results.Ok(fileNames);
        }
        catch (Exception e)
        {
            // TODO: in production, find way to return 500 Internal server error
            return Results.BadRequest($"Failed to save document: {e.Message}");
        }
    }
}
