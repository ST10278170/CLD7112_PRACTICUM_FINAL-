using AgriStreamIoT.Models;
using Azure.Storage.Queues;
using System.Text.Json;

namespace AgriStreamIoT.Services
{
    public class IrrigationQueueService
    {
        private readonly QueueClient _queueClient;

        public IrrigationQueueService(
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string was not found.");

            string queueName =
                configuration["AzureStorage:IrrigationQueueName"]
                ?? "irrigation-commands";

            _queueClient = new QueueClient(
                connectionString,
                queueName);
        }

        public async Task SendCommandAsync(
            IrrigationCommand command)
        {
            await _queueClient.CreateIfNotExistsAsync();

            command.CommandId = Guid.NewGuid();
            command.RequestedAt = DateTimeOffset.UtcNow;

            string message =
                JsonSerializer.Serialize(command);

            await _queueClient.SendMessageAsync(message);
        }
    }
}