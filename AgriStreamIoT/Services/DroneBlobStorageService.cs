using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AgriStreamIoT.Services
{
    public class DroneBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public DroneBlobStorageService(
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string was not found.");

            string containerName =
                configuration["AzureStorage:DroneImageContainerName"]
                ?? "drone-images";

            _containerClient =
                new BlobContainerClient(
                    connectionString,
                    containerName);
        }

        public async Task<string> UploadDroneImageAsync(
            IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException(
                    "A valid drone image is required.");
            }

            await _containerClient.CreateIfNotExistsAsync();

            string extension =
                Path.GetExtension(image.FileName);

            string blobName =
                $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            using Stream stream =
                image.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                });

            return blobClient.Uri.ToString();
        }
    }
}