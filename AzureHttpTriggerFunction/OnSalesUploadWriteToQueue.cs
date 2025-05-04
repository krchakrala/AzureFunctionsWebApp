using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace AzureHttpTriggerFunction
{

    public class OnSalesUploadWriteToQueue
    {
        private readonly ILogger<OnSalesUploadWriteToQueue> _logger;
        public OnSalesUploadWriteToQueue(ILogger<OnSalesUploadWriteToQueue> logger)
        {
            _logger = logger;
        }
        [Function("OnSalesUploadWriteToQueue")]
        [QueueOutput("salesrequestinbound", Connection = "AzureWebJobsStorage")]
        public async Task<SalesRequest> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
        FunctionContext context)
        {
            _logger.LogInformation("Sales Request received by OnSalesUploadWriteToQueue function");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SalesRequest? responseData = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            
            //Optional: Validate/Deserialize the request body
            if (string.IsNullOrWhiteSpace(requestBody))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Empty request body.");
                return null!;
            }
            _logger.LogInformation("Queueing message...");
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync("Sales Request has been received to Queue");
            return responseData;
        }
    } 
}
