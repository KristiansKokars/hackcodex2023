using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class AzureFileService
{
    public static async Task<string> SaveFileToStorage(IFormFile file)
    {
        // TODO: add your_storage_account_connection_string
        string connectionString = "your_storage_account_connection_string";
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

        // TODO: add your_container_name
        string containerName = "your_container_name";
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