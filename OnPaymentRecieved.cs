using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure_Functions
{
    // static class
    public static class OnPaymentRecieved
    {
        // static function Run()
        [FunctionName("OnPaymentRecieved")]  // function name attribute
        public static async Task<IActionResult> Run(
            // http trigger attribute
            // authorization level = function - req secret code
            // which http methods we want to support
            // Default Route api/functionName, but we can use Route param to configure different Route for fn invocation
            // ILogger write to the logs
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("orders")] IAsyncCollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("Recieved the Payment");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(requestBody);
            
            // pass the order to queue
            await orderQueue.AddAsync(order);

            log.LogInformation($"Order {order.OrderId} received from {order.Email} for prodcut {order.ProductId}");

            return new OkObjectResult("Thank you for your purchase");
        }
    }

    public class Order {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}
