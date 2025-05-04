using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace AzureHttpTriggerFunction
{
    public static class HttpTriggerFuncObject
    {
        public class SomeResult
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        [Function("HttpTriggerFuncObject")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                   ILogger log,
                   ExecutionContext executionContext)
        {
            string name = req.Query["name"];
            string email = req.Query["email"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            email = email ?? data?.email;

            // return name != null ? CreateResponse():CreateResponse(); 

            return name != null
                       ? CreateResponse(HttpStatusCode.OK, new SomeResult
                       {
                           Name = name,
                           Email = email
                       })
                       : CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
        }
        public static IActionResult CreateResponse(HttpStatusCode code, object content)
        {
            ContentResult result = new ContentResult();
            result.Content = JsonConvert.SerializeObject(content, Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            result.ContentType = "application/json";
            result.StatusCode = (int)code;
            return result;
        }

    }
}
