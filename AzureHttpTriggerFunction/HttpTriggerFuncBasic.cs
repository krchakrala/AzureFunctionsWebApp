using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;

namespace AzureHttpTriggerFunction
{
    public static class HttpTriggerFuncBasic
    {
        public class SomeResult
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
        //Function 1
        [Function("HttpTriggerFuncBasic")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
           ILogger log)
        {
            //return new OkObjectResult("welcome to Function app");
            string name = req.Query["name"];
            string email = req.Query["email"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            email = email ?? data?.email;
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed Successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Please pass a {name} - {email} on the query string or in the request body";
            return new OkObjectResult(responseMessage);
        }

    }
}
