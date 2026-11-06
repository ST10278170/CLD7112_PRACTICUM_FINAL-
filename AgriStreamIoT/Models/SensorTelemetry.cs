using Azure;
using Azure.Data.Tables;

namespace AgriStreamIoT.Models
{
    public class SensorTelemetry : ITableEntity
    {
        public string PartitionKey { get; set; } = "FieldTelemetry";

        public string RowKey { get; set; } =
            Guid.NewGuid().ToString();

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public int DeviceId { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string FieldLocation { get; set; } = string.Empty;

        public double SoilMoisture { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public string IrrigationStatus { get; set; } = "Inactive";

        public DateTimeOffset RecordedAt { get; set; } =
            DateTimeOffset.UtcNow;
    }
}