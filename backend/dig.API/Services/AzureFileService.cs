using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using dig.API.Common;
using dig.API.Feature.Auth;

public class AzureFileService
{
    public static async Task<Either<SimpleMessageError, string>> SaveFileToStorage(IFormFile file)
    {
        var connectionString = Environment.GetEnvironmentVariable("FR_BLOB_CONNECTION_STRING");
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        var containerName = Environment.GetEnvironmentVariable("FR_CONTAINER");
        if (containerName is null)
        {
            return new Either<SimpleMessageError, string>(new SimpleMessageError("Failed to retrieve container name from ENV!"));
        }

        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Generate a unique file name
        var extension = Path.GetExtension(file.FileName);
        var fileName = Guid.NewGuid().ToString() + extension;
        var contentType = "application/pdf";
        if (extension != ".pdf")
        {
            contentType = "application/image";
        }

        Console.WriteLine("CONTENT TYPE: " + contentType);
        
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        using (Stream fileStream = file.OpenReadStream())
        {
            var uploadResult = await blobClient.UploadAsync(fileStream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            }); // , overwrite: true
            if (uploadResult is null)
            {
                return new Either<SimpleMessageError, string>(new SimpleMessageError("Failed to upload document!"));
            }
        }

        Uri blobUri = blobClient.Uri;
        return new Either<SimpleMessageError, string>(blobUri.ToString());
    }

    public static async Task<Either<SimpleMessageError, string>> RetrieveJSONFromStorage(string fileName)
    {
        // TODO: return back in prod

        // var connectionString = Environment.GetEnvironmentVariable("FR_BLOB_CONNECTION_STRING");
        // BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        // var containerName = Environment.GetEnvironmentVariable("FR_CONTAINER");
        // if (containerName is null)
        // {
        //     return new Either<SimpleMessageError, string>(new SimpleMessageError("Failed to retrieve container name from ENV!"));
        // }

        // BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        // BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // var response = await blobClient.DownloadAsync();
        // if (response is null)
        // {
        //     return new Either<SimpleMessageError, string>(new SimpleMessageError("Failed to download JSON document!"));
        // }

        // using StreamReader streamReader = new StreamReader(response.Value.Content);
        // var jsonContent = await streamReader.ReadToEndAsync();

        // return new Either<SimpleMessageError, string>(jsonContent);
        // TODO: remove in prod

        return new Either<SimpleMessageError, string>("hello");
    }
}