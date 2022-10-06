using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Azure_Functions
{
    public class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public void Run([QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order, ILogger log,
            [Blob("licenses/{rand-guid}.lic")] TextWriter outputBlog)
        {
            outputBlog.WriteLine($"Order Id : {order.OrderId}");
            outputBlog.WriteLine($"Email: {order.Email}");
            outputBlog.WriteLine($"Product Id : {order.ProductId}");
            outputBlog.WriteLine($"Purchase Date : {DateTime.UtcNow}");

            var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));

            outputBlog.WriteLine($"Secret Code : {BitConverter.ToString(hash).Replace("-","")}");

            log.LogInformation($"C# Queue trigger function processed: {order}");
        }
    }
}
