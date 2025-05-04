using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace AzureHttpTriggerFunction
{
    public class OnSalesUploadWriteToQueueBlob
    {
        private readonly ILogger<OnSalesUploadWriteToQueueBlob> _logger;
        private readonly IQueueService _queueService;
        public OnSalesUploadWriteToQueueBlob(ILogger<OnSalesUploadWriteToQueueBlob> logger, IQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
        }
        [Function("OnSalesUploadWriteToQueue")]
        public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
        //[QueueOutput("SalesRequestInBound", Connection = "AzureWebJobsStorage")] IAsyncCollector<SalesRequest> salesRequestQueue,
        FunctionContext context)
        {
            _logger.LogInformation("Sales Request received by OnSalesUploadWriteToQueue function");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            await _queueService.AddMessageToQueueAsync("SalesRequestInBound", data);
            var reponse = req.CreateResponse(HttpStatusCode.OK);
            await reponse.WriteStringAsync($"Sales Request has been received for {data.Name}");
            return reponse;
            //string responseMessage = "Sales Request has been received for ." + data.Name;
            //return new OkObjectResult(responseMessage);
        }

    }
    public class SalesRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }
    public interface IQueueService
    {
        Task AddMessageToQueueAsync<T>(string queueName, T message);
    }
    public class QueueService : IQueueService
    {
        private readonly QueueClientFactory _queueClientFactory;
        public QueueService(QueueClientFactory queueClientFactory)
        {
            _queueClientFactory = queueClientFactory;
        }
        public async Task AddMessageToQueueAsync<T>(string queueName, T message)
        {
            var _queueClient = _queueClientFactory.CreateQueueClient(queueName);
            var jsonMessageContent = JsonConvert.SerializeObject(message);
            await _queueClient.SendMessageAsync(jsonMessageContent);
        }
    }
    public class QueueClientFactory
    {
        private readonly string _connectionString;
        public QueueClientFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public QueueClient CreateQueueClient(string queueName)
        {
            var queueClient = new QueueClient(_connectionString, queueName);
            queueClient.CreateIfNotExists();
            return queueClient;
        }
    }
    //public static class OnSalesUploadWriteToQueue
    //{
    //    [Function("OnSalesUploadWriteToQueue")]
    //    public static async Task<IActionResult> Run(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
    //    [QueueOutput("SalesRequestInBound", Connection = "AzureWebJobsStorage")] IAsyncCollector<SalesRequest> salesRequestQueue,   
    //    ILogger log)
    //    {
    //       // log.LogInformation("Sales Request received by OnSalesUploadWriteToQueue function");

    //        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    //        SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
    //        if (data.Id == null)
    //        {
    //            throw new Exception("salesRequestQueue is null. Check binding configuration.");
    //        }
    //        await salesRequestQueue.AddAsync(data);

    //        string responseMessage = "Sales Request has been received for ." + data.Name;
    //        return new OkObjectResult(responseMessage);
    //    }
    //} 
}
