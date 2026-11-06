using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AgriStreamFunctions
{
    public class LatestTelemetryFunction
    {
        private readonly ILogger<LatestTelemetryFunction> _logger;

        public LatestTelemetryFunction(
            ILogger<LatestTelemetryFunction> logger)
        {
            _logger = logger;
        }

        [Function("LatestTelemetryFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "get",
                Route = "telemetry/latest")]
            HttpRequestData req)
        {
            _logger.LogInformation(
                "Latest telemetry request received.");

            string connectionString =
                Environment.GetEnvironmentVariable(
                    "AzureWebJobsStorage")
                ?? throw new InvalidOperationException(
                    "AzureWebJobsStorage is not configured.");

            var tableClient = new TableClient(
                connectionString,
                "SensorTelemetry");

            await tableClient.CreateIfNotExistsAsync();

            var readings = new List<TableEntity>();

            await foreach (
                TableEntity entity in
                tableClient.QueryAsync<TableEntity>())
            {
                readings.Add(entity);
            }

            TableEntity? latest =
                readings
                    .OrderByDescending(entity =>
                        entity.GetDateTimeOffset("RecordedAt"))
                    .FirstOrDefault();

            if (latest == null)
            {
                var notFoundResponse =
                    req.CreateResponse(HttpStatusCode.NotFound);

                await notFoundResponse.WriteStringAsync(
                    "No telemetry readings are available.");

                return notFoundResponse;
            }

            var result = new
            {
                DeviceId =
                    latest.GetInt32("DeviceId"),

                DeviceName =
                    latest.GetString("DeviceName"),

                FieldLocation =
                    latest.GetString("FieldLocation"),

                SoilMoisture =
                    latest.GetDouble("SoilMoisture"),

                Temperature =
                    latest.GetDouble("Temperature"),

                Humidity =
                    latest.GetDouble("Humidity"),

                IrrigationStatus =
                    latest.GetString("IrrigationStatus"),

                RecordedAt =
                    latest.GetDateTimeOffset("RecordedAt")
            };

            var response =
                req.CreateResponse(HttpStatusCode.OK);

            response.Headers.Add(
                "Content-Type",
                "application/json; charset=utf-8");

            await response.WriteStringAsync(
                JsonSerializer.Serialize(
                    result,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            _logger.LogInformation(
                "Latest telemetry returned successfully.");

            return response;
        }
    }
}