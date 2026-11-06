using AgriStreamFunctions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AgriStreamFunctions
{
    public class DroneImageBlobFunction
    {
        private readonly ILogger<DroneImageBlobFunction> _logger;

        public DroneImageBlobFunction(
            ILogger<DroneImageBlobFunction> logger)
        {
            _logger = logger;
        }

        [Function("DroneImageBlobFunction")]
        public async Task Run(
            [BlobTrigger(
                "drone-images/{name}",
                Connection = "AzureWebJobsStorage")]
            byte[] blobContent,
            string name)
        {
            _logger.LogInformation(
                "New agricultural drone image detected.");

            _logger.LogInformation(
                "Drone image: {ImageName}",
                name);

            _logger.LogInformation(
                "Image size: {ImageSize} bytes",
                blobContent.Length);

            string connectionString =
                Environment.GetEnvironmentVariable(
                    "AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage is not configured.");

            var tableClient = new TableClient(
                connectionString,
                "DroneImageProcessing");

            await tableClient.CreateIfNotExistsAsync();

            var processingLog =
                new DroneImageProcessingLog
                {
                    PartitionKey = "DroneImageProcessing",
                    RowKey = Guid.NewGuid().ToString(),
                    ImageName = name,
                    ImageSize = blobContent.LongLength,
                    ProcessingStatus = "Processed",
                    Message =
                        "Drone image detected and processed successfully.",
                    ProcessedAt = DateTimeOffset.UtcNow
                };

            await tableClient.AddEntityAsync(processingLog);

            _logger.LogInformation(
                "Drone image processing status written to Table Storage.");
        }
    }
}