using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using watchStewar.Common.Responses;
using watchStewar.Functions.Entities;

namespace watchStewar.Functions.Functions
{
    public static class ConsolidateAPI
    {
        [FunctionName(nameof(GetConsolidateByDate))]
        public static async Task<IActionResult> GetConsolidateByDate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidate/{date}")] HttpRequest req,
            [Table("ConsolidatedRegisters", Connection = "AzureWebJobsStorage")] CloudTable consolidateTable,
            string date,
            ILogger log)
        {
            log.LogInformation($"Getting all the consolidates for day {date}.");

            DateTime iniDay = DateTime.Parse(date + " 00:00 ");
            DateTime endDay = DateTime.Parse(date + " 23:59 ");
            TableQuerySegment<ConsolidateEntity> consolidates = await consolidateTable.ExecuteQuerySegmentedAsync(new TableQuery<ConsolidateEntity>(), null);
            List<ConsolidateEntity> consolidateList = new List<ConsolidateEntity>();

            foreach (ConsolidateEntity consolidate in consolidates)
            {
                if (consolidate.date >= iniDay && consolidate.date <= endDay)
                {
                    consolidateList.Add(consolidate);
                }
            }


            string message = $"Consolidates for date: {date} retrieved";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                isSuccess = true,
                message = message,
                result = consolidateList
            });
        }
    }
}
