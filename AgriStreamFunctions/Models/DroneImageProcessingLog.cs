using Azure;
using Azure.Data.Tables;

namespace AgriStreamFunctions.Models
{
    public class DroneImageProcessingLog : ITableEntity
    {
        public string PartitionKey { get; set; } =
            "DroneImageProcessing";

        public string RowKey { get; set; } =
            Guid.NewGuid().ToString();

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public string ImageName { get; set; } =
            string.Empty;

        public long ImageSize { get; set; }

        public string ProcessingStatus { get; set; } =
            "Processed";

        public string Message { get; set; } =
            string.Empty;

        public DateTimeOffset ProcessedAt { get; set; } =
            DateTimeOffset.UtcNow;
    }
}