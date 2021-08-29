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

            Console.WriteLine(watch?.idWorker.ToString());

            if (string.IsNullOrEmpty(watch?.idWorker.ToString()) || watch?.idWorker <= 0)
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

            if (string.IsNullOrEmpty(watch?.register.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Invalid request, date is required "
                });
            }

            WatchEntity watchEntity = new WatchEntity
            {
                idWorker = watch.idWorker,
                register = watch.register,
                type = watch.type,
                isConsolidate = false,
                ETag = "*",
                PartitionKey = "RegisterTable",
                RowKey = Guid.NewGuid().ToString()
            };

            TableOperation createOperation = TableOperation.Insert(watchEntity);
            await watchTable.ExecuteAsync(createOperation);

            string message = "New register created successfully & stored in table";
            log.LogInformation("New register created successfully & stored in table.");

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = watchEntity
            });
        }

        [FunctionName(nameof(UpdateRegisterById))]
        public static async Task<IActionResult> UpdateRegisterById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "watch/{id}")] HttpRequest req,
            [Table("RegisterTable", Connection = "AzureWebJobsStorage")] CloudTable watchTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Updating entrance to register for worker {id}.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Watch watch = JsonConvert.DeserializeObject<Watch>(requestBody);

            Console.WriteLine(watch?.idWorker.ToString());

            // Validation for the id
            TableOperation findId = TableOperation.Retrieve<WatchEntity>("RegisterTable", id);
            TableResult findResult = await watchTable.ExecuteAsync(findId);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = $"The id: {id} isn't a valid id, please check."
                });
            }
            // Updating watch
            WatchEntity watchEntity = (WatchEntity)findResult.Result;
            if (!string.IsNullOrEmpty(watch.register.ToString()))
            {
                watchEntity.register = watch.register;
            }

            TableOperation updateOperation = TableOperation.Replace(watchEntity);
            await watchTable.ExecuteAsync(updateOperation);

            string message = $"Updated date in table for worker with id: {id}";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = watchEntity
            });
        }

        [FunctionName(nameof(GetAllRegisters))]
        public static async Task<IActionResult> GetAllRegisters(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "watch")] HttpRequest req,
            [Table("RegisterTable", Connection = "AzureWebJobsStorage")] CloudTable watchTable,
            ILogger log)
        {
            log.LogInformation("Getting all the information.");

            TableQuery<WatchEntity> query = new TableQuery<WatchEntity>();
            TableQuerySegment<WatchEntity> watches = await watchTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieving all the watches";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = watches
            });
        }


        [FunctionName(nameof(GetRegisterById))]
        public static IActionResult GetRegisterById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "watch/{id}")] HttpRequest req,
            [Table("RegisterTable", "RegisterTable", "{id}", Connection = "AzureWebJobsStorage")] WatchEntity watchEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Getting information for id: {id}.");

            //Validate ID
            if (watchEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    isSuccess = false,
                    message = "Register not found."
                });
            }

            

            string message = $"Register with id {id} retrieved";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = watchEntity
            });
        }

    }
}
