using System.Text.Json;
using AgriStreamFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace AgriStreamFunctions
{
    public class ProcessIrrigationQueueFunction
    {
        private readonly ILogger<ProcessIrrigationQueueFunction> _logger;

        public ProcessIrrigationQueueFunction(
            ILogger<ProcessIrrigationQueueFunction> logger)
        {
            _logger = logger;
        }

        [Function("ProcessIrrigationQueueFunction")]
        public async Task Run(
            [QueueTrigger(
                "irrigation-commands",
                Connection = "AzureWebJobsStorage")]
            string queueMessage)
        {
            _logger.LogInformation(
                "Irrigation queue message received.");

            IrrigationCommandMessage? command;

            try
            {
                command =
                    JsonSerializer.Deserialize<IrrigationCommandMessage>(
                        queueMessage,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
            }
            catch (JsonException ex)
            {
                _logger.LogError(
                    ex,
                    "The irrigation queue message contains invalid JSON.");

                throw;
            }

            if (command == null || command.DeviceId <= 0)
            {
                throw new InvalidOperationException(
                    "The irrigation queue message does not contain " +
                    "a valid DeviceId.");
            }

            string connectionString =
                Environment.GetEnvironmentVariable(
                    "SqlConnectionString")
                ?? throw new InvalidOperationException(
                    "SqlConnectionString is not configured.");

            string operationalStatus =
                command.Command.Equals(
                    "StartIrrigation",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Irrigating"
                    : command.Command.Equals(
                        "StopIrrigation",
                        StringComparison.OrdinalIgnoreCase)
                        ? "Online"
                        : throw new InvalidOperationException(
                            $"Unsupported irrigation command: " +
                            $"{command.Command}");

            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            const string sql = """
                UPDATE Devices
                SET OperationalStatus = @OperationalStatus,
                    LastUpdated = @LastUpdated
                WHERE Id = @DeviceId;
                """;

            await using var sqlCommand =
                new SqlCommand(sql, connection);

            sqlCommand.Parameters.AddWithValue(
                "@OperationalStatus",
                operationalStatus);

            sqlCommand.Parameters.AddWithValue(
                "@LastUpdated",
                DateTime.UtcNow);

            sqlCommand.Parameters.AddWithValue(
                "@DeviceId",
                command.DeviceId);

            int affectedRows =
                await sqlCommand.ExecuteNonQueryAsync();

            if (affectedRows == 0)
            {
                throw new InvalidOperationException(
                    $"No device with ID {command.DeviceId} " +
                    "was found in the SQL device registry.");
            }

            _logger.LogInformation(
                "Irrigation command {CommandId} processed. " +
                "Device {DeviceId} status updated to {Status}.",
                command.CommandId,
                command.DeviceId,
                operationalStatus);
        }
    }
}