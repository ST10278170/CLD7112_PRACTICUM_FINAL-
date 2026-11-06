using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AgriStreamFunctions
{
    public class QueueHttpResponse
    {
        public HttpResponseData HttpResponse { get; set; } = null!;

        [QueueOutput(
            "irrigation-commands",
            Connection = "AzureWebJobsStorage")]
        public string? QueueMessage { get; set; }
    }
}