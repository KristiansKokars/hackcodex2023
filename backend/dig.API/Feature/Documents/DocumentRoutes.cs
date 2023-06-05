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
        ClaimsPrincipal user,
        DocumentService docService)
    {
        Console.WriteLine($"Document id: {id}");
        var documentId = Guid.Parse(id);

        // Retrieve user
        var userId = user.Claims.First(claim => claim.Type == "Id").Value;

        // Read the request body
        using var reader = new StreamReader(context.Request.Body);
        var requestBody = await reader.ReadToEndAsync();
        Console.WriteLine("RESOLVE REQ BODY:");
        Console.WriteLine(requestBody);

        try
        {
            // var docContents = JsonSerializer.Deserialize<DocContentDto>(requestBody);
            // Console.WriteLine("RESOLVE REQUEST:");
            
            // // TODO: request body is empty
            // var contents = JsonSerializer.Serialize(docContents);
            // Console.WriteLine(contents);

            // TODO: evaluate if format correct

            var resolved = docService.MarkDocumentSolved(
                Guid.Parse(userId), 
                documentId, 
                requestBody);

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
        double minRequiredPrecison = 0.85;

        // Retrieve documents
        var docs = documentService.GetDocuments(Guid.Parse(userId));

        Console.WriteLine("DOCS: " + docs.Count);

        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            // TODO: recheck
            // InvoiceId = JsonSerializer.Deserialize<DocIdDto>(d.Content)!.InvoiceId["Value"].ToString()!,
            InvoiceId = d.InvoiceId,
            Content = JsonSerializer.Deserialize<ResponseDocContentDto>(d.Content)!,
            Link = d.Link,
            Status = d.Status,
            CreatedAt = d.CreatedAt != null 
                ? Helpers.ConvertToUnixMillis(d.CreatedAt) 
                : Helpers.ConvertToUnixMillis(DateTime.Now),
            MinAllowedPercent = minRequiredPrecison,
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
        double minRequiredPrecison = 0.85;

        // Retrieve faulty documents
        var docs = documentService.GetFaultyDocuments(Guid.Parse(userId));
        var documentDtos = docs.Select(d => new DocumentDto
        {
            Id = d.Id,
            // InvoiceId = JsonSerializer.Deserialize<DocIdDto>(d.Content)!.InvoiceId["Value"].ToString()!,
            InvoiceId = d.InvoiceId,
            Content = JsonSerializer.Deserialize<ResponseDocContentDto>(d.Content)!,
            Link = d.Link,
            Status = d.Status,
            CreatedAt = d.CreatedAt != null 
                ? Helpers.ConvertToUnixMillis(d.CreatedAt) 
                : Helpers.ConvertToUnixMillis(DateTime.Now),
            MinAllowedPercent = minRequiredPrecison,
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
                InvoiceId = JsonSerializer.Deserialize<DocIdDto>(document.Content)!.InvoiceId["Value"].ToString()!,
                Content = JsonSerializer.Deserialize<ResponseDocContentDto>(document.Content)!,
                Link = document.Link,
                Status = document.Status,
                CreatedAt = document.CreatedAt != null 
                    ? Helpers.ConvertToUnixMillis(document.CreatedAt) 
                    : Helpers.ConvertToUnixMillis(DateTime.Now),
                MinAllowedPercent = minRequiredPrecison,
            };

            return Results.Ok(documentDto);
        }
        else
        {
            var labelsLink = document.Link + ".labels.json";

            // TODO: resolve why document not found
            // TODO: remove workaround in prod
            var labelsJSONResult = await AzureFileService.RetrieveJSONFromStorage(labelsLink);
            return labelsJSONResult.Map<IResult>(
                error: error => 
                {
                    // TODO: in production, find way to return 500 Internal server error
                    return Results.BadRequest(error);
                },
                // TODO: remember about 'hello'
                success: labelsJSON =>
                {
                    // var labelDocument = JsonSerializer.Deserialize<DocLabelsDto>(labelsJSON);
                    var documentContent = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(document.Content);

                    // TODO: remove in prod
                    Console.WriteLine("RETRIEVED CONTENT:");
                    Console.WriteLine(document.Content);
                    Console.WriteLine();

                    string? invoiceId = null;

                    var documentContentMarked = new Dictionary<string, Dictionary<string, object>>();
                    foreach (var documentRow in documentContent!)
                    {
                        var name = documentRow.Key;
                        var rowContent = documentRow.Value;
                        var fieldValue = rowContent["Value"];
                        var fieldConfidence = Convert.ToDouble(float.Parse(rowContent["Confidence"].ToString()));

                        if (name == "InvoiceId")
                        {
                            invoiceId = fieldValue.ToString();
                        }
                        else
                        {
                            // TODO: implement and resolve label issues

                            // if (fieldConfidence < minRequiredPrecison)
                            // {
                            //     var documentFieldValue = new Dictionary<string, object>();
                            //     documentFieldValue.Add("Value", fieldValue);
                            //     documentFieldValue.Add("Confidence", fieldConfidence);
                    
                            //     var fieldLabels = labelDocument!.Labels
                            //         .SelectMany(label => label.Value)
                            //         .SelectMany(labelList => labelList.BoundingBoxes)
                            //         .Select(box => box)
                            //         .ToList();
                    
                            //     documentFieldValue.Add("Labels", fieldLabels);

                            //     documentContentMarked.Add(name, documentFieldValue);
                            // }
                            // else
                            // {
                            documentContentMarked.Add(documentRow.Key, documentRow.Value);
                            // }
                            Console.WriteLine($"Row: {name}, Value: {fieldValue}, %: {fieldConfidence}");
                        }
                    }

                    var contentMarkedJSON = JsonSerializer.Serialize(documentContentMarked);

                    var documentDtoMarked = new DocumentDto
                    {
                        Id = document.Id,
                        InvoiceId = invoiceId!,
                        Content = JsonSerializer.Deserialize<ResponseDocContentDto>(contentMarkedJSON)!,
                        Link = document.Link,
                        Status = document.Status,
                        CreatedAt = document.CreatedAt != null 
                            ? Helpers.ConvertToUnixMillis(document.CreatedAt) 
                            : Helpers.ConvertToUnixMillis(DateTime.Now),
                        MinAllowedPercent = minRequiredPrecison,
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
            Dictionary<string, Dictionary<string, object>> req = new Dictionary<string, Dictionary<string, object>>();
            try
            {
                req = await AzureAIService.RecognizeInvoiceModel(fileName);
            }
            catch (Exception e)
            {
                // TODO: in production, find way to return 500 Internal server error
                Console.WriteLine(e);
                return Results.BadRequest(e.Message);
            }

            Console.WriteLine("OUTPUT FROM RECOGNITION: \n");
            Console.WriteLine(req);

            var docStatus = "Correct";

            string? invoiceId = null;

            foreach (var documentRow in req)
            {
                var name = documentRow.Key;
                var rowContent = documentRow.Value;

                var fieldValue = rowContent["Value"];
                var fieldConfidence = Convert.ToDouble((float)rowContent["Confidence"]);

                if (name == "InvoiceId")
                {
                    invoiceId = fieldValue.ToString();
                }
                else 
                {
                    if (fieldConfidence < minRequiredPrecison)
                    {
                        docStatus = "Faulty";
                        Console.WriteLine("Saving a faulty document");
                    }

                    Console.WriteLine($"Row: {name}, Value: {fieldValue}, %: {fieldConfidence}");
                }
            }

            var content = JsonSerializer.Serialize(req);
            var doc = documentService.DocumentHelper(content, Guid.Parse(userId), fileName, docStatus);
            doc.InvoiceId = invoiceId!;

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
