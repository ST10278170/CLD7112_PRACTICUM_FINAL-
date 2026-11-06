using AgriStreamIoT.Models;
using Azure.Data.Tables;

namespace AgriStreamIoT.Services
{
    public class TelemetryTableService
    {
        private readonly TableClient _tableClient;

        public TelemetryTableService(
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string was not found.");

            string tableName =
                configuration["AzureStorage:TelemetryTableName"]
                ?? "SensorTelemetry";

            _tableClient = new TableClient(
                connectionString,
                tableName);
        }

        public async Task AddTelemetryAsync(
            SensorTelemetry telemetry)
        {
            await _tableClient.CreateIfNotExistsAsync();

            telemetry.PartitionKey =
                string.IsNullOrWhiteSpace(telemetry.FieldLocation)
                    ? "FieldTelemetry"
                    : telemetry.FieldLocation.Replace(" ", "");

            telemetry.RowKey =
                Guid.NewGuid().ToString();

            telemetry.RecordedAt =
                DateTimeOffset.UtcNow;

            await _tableClient.AddEntityAsync(telemetry);
        }

        public async Task<List<SensorTelemetry>>
            GetTelemetryAsync()
        {
            await _tableClient.CreateIfNotExistsAsync();

            var readings =
                new List<SensorTelemetry>();

            await foreach (
                SensorTelemetry reading in
                _tableClient.QueryAsync<SensorTelemetry>())
            {
                readings.Add(reading);
            }

            return readings
                .OrderByDescending(
                    reading => reading.RecordedAt)
                .ToList();
        }

        public async Task<SensorTelemetry?>
            GetLatestTelemetryAsync()
        {
            var readings =
                await GetTelemetryAsync();

            return readings.FirstOrDefault();
        }
    }
}