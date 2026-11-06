using System.Net;
using System.Text.Json;
using AgriStreamFunctions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AgriStreamFunctions
{
    public class TriggerIrrigationFunction
    {
        private readonly ILogger<TriggerIrrigationFunction> _logger;

        public TriggerIrrigationFunction(
            ILogger<TriggerIrrigationFunction> logger)
        {
            _logger = logger;
        }

        [Function("TriggerIrrigationFunction")]
        public async Task<QueueHttpResponse> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "post",
                Route = "irrigation/trigger")]
            HttpRequestData req)
        {
            _logger.LogInformation(
                "Irrigation HTTP POST request received.");

            string requestBody =
                await new StreamReader(req.Body)
                    .ReadToEndAsync();

            IrrigationCommandMessage? command;

            try
            {
                command =
                    JsonSerializer.Deserialize<IrrigationCommandMessage>(
                        requestBody,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
            }
            catch (JsonException)
            {
                var invalidJsonResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await invalidJsonResponse.WriteStringAsync(
                    "The request body contains invalid JSON.");

                return new QueueHttpResponse
                {
                    HttpResponse = invalidJsonResponse
                };
            }

            if (command == null ||
                command.DeviceId <= 0 ||
                string.IsNullOrWhiteSpace(command.Command))
            {
                var badResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await badResponse.WriteStringAsync(
                    "DeviceId and Command are required.");

                return new QueueHttpResponse
                {
                    HttpResponse = badResponse
                };
            }

            command.CommandId = Guid.NewGuid();
            command.RequestedAt = DateTimeOffset.UtcNow;

            if (string.IsNullOrWhiteSpace(command.RequestedBy))
            {
                command.RequestedBy =
                    "Azure Function";
            }

            string queueMessage =
                JsonSerializer.Serialize(command);

            var response =
                req.CreateResponse(HttpStatusCode.Accepted);

            response.Headers.Add(
                "Content-Type",
                "application/json; charset=utf-8");

            var result = new
            {
                command.CommandId,
                command.DeviceId,
                command.Command,
                Status = "Queued",
                Message =
                    "Irrigation command accepted for asynchronous processing."
            };

            await response.WriteStringAsync(
                JsonSerializer.Serialize(
                    result,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            _logger.LogInformation(
                "Irrigation command {CommandId} queued for device {DeviceId}.",
                command.CommandId,
                command.DeviceId);

            return new QueueHttpResponse
            {
                HttpResponse = response,
                QueueMessage = queueMessage
            };
        }
    }
}