using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using watchStewar.Common.Models;
using watchStewar.Common.Responses;
using watchStewar.Functions.Entities;

namespace watchStewar.Functions.Functions
{
    public static class WatchAPI
    {
        [FunctionName(nameof(CreateRegister))]
        public static async Task<IActionResult> CreateRegister(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "watch")] HttpRequest req,
            [Table("RegisterTable", Connection = "AzureWebJobsStorage")] CloudTable watchTable,
            ILogger log)
        {
            log.LogInformation("Creating a new register.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Watch watch = JsonConvert.DeserializeObject<Watch>(requestBody);

            if (watch?.idWorker == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Invalid request, id of worker can't be blank"
                });
            }

            

            if (string.IsNullOrEmpty(watch?.type.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Invalid request, type of entrace or way out is required "
                });
            }

            WatchEntity watchEntity = new WatchEntity
            {
                idWorker = watch.idWorker,
                register = DateTime.UtcNow,
                type = watch.type,
                isConsolidate = false,
                ETag = "*",
                PartitionKey = "RegisterTable",
                RowKey = Guid.NewGuid().ToString()
            };

            TableOperation createOperation = TableOperation.Insert(watchEntity);
            await watchTable.ExecuteAsync(createOperation);

            string message = "New register created successfully & stored in table";

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = watchEntity
            });
        }
    }
}
