using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using watchStewar.Functions.Entities;

namespace watchStewar.Functions.Functions
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Run([TimerTrigger("*/1 * * * *")] TimerInfo myTimer,
            [Table("RegisterTable", Connection = "AzureWebJobsStorage")] CloudTable watchTable,
            [Table("ConsolidatedRegisters", Connection = "AzureWebJobsStorage")] CloudTable consolidateTable,
            ILogger log)
        {
            log.LogInformation($"Consolidating data in table consolidate");

            string filter = TableQuery.GenerateFilterConditionForBool("isConsolidate", QueryComparisons.Equal, false);
            TableQuery<WatchEntity> query = new TableQuery<WatchEntity>().Where(filter);
            TableQuerySegment<WatchEntity> consolidatesRegisters = await watchTable.ExecuteQuerySegmentedAsync(query, null);
            int consolidateData = 0;

            foreach (WatchEntity consolidateRegister in consolidatesRegisters) {
                WatchEntity watchEntity = consolidateRegister;
                watchEntity.isConsolidate = true;
                await watchTable.ExecuteAsync(TableOperation.Replace(watchEntity));
                consolidateData++;
            }
            log.LogInformation($"{consolidateData} was counted");
        }
    }
}
