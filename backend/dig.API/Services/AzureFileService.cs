using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class AzureFileService
{
    public static async Task<string> SaveFileToStorage(IFormFile file)
    {
        var connectionString = Environment.GetEnvironmentVariable("FR_BLOB_CONNECTION_STRING");
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        string containerName = Environment.GetEnvironmentVariable("FR_CONTAINER");
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Generate a unique file name
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        using (Stream fileStream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        Uri blobUri = blobClient.Uri;

        return blobUri.ToString();
    }

    public static async Task<string> RetrieveJSONFromStorage(string fileName)
    {
        var connectionString = Environment.GetEnvironmentVariable("FR_BLOB_CONNECTION_STRING");
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        string containerName = Environment.GetEnvironmentVariable("FR_CONTAINER");
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        var response = await blobClient.DownloadAsync();
        using StreamReader streamReader = new StreamReader(response.Value.Content);
        string jsonContent = await streamReader.ReadToEndAsync();

        return jsonContent;
    }
}