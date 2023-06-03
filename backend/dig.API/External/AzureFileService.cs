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

        // Create a BlobClient instance for the new blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Upload the file to Azure Blob Storage
        using (Stream fileStream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        // Get the URL of the uploaded blob
        Uri blobUri = blobClient.Uri;

        return blobUri.ToString();
    }
}