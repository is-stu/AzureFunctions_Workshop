using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
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
            TableQuerySegment<WatchEntity> unconsolidatesRegisters = await watchTable.ExecuteQuerySegmentedAsync(query, null);
            int totalAdded = 0;
            int totalUpdated = 0;

            //Set by id
            List<IGrouping<int, WatchEntity>> groupUnconsolidate = unconsolidatesRegisters.GroupBy(u => u.idWorker).OrderBy(u => u.Key).ToList();
            foreach (IGrouping<int, WatchEntity> group in groupUnconsolidate)
            {
                TimeSpan difference;
                double totalMinutes = 0;
                List<WatchEntity> orderedRegisters = group.OrderBy(g => g.register).ToList();
                int isEven = orderedRegisters.Count % 2 == 0 ? orderedRegisters.Count : orderedRegisters.Count - 1;
                WatchEntity[] watchesAuxiliar = orderedRegisters.ToArray();
                try
                {
                    for (int i = 0; i < isEven; i++)
                    {
                        await ChangeConsolidateStatus(watchesAuxiliar[i].RowKey, watchTable);
                        if (i % 2 != 0 && watchesAuxiliar.Length > 1)
                        {
                            difference = watchesAuxiliar[i].register - watchesAuxiliar[i - 1].register;
                            totalMinutes += difference.TotalMinutes;
                            TableQuerySegment<ConsolidateEntity> allConsolidated = await consolidateTable.ExecuteQuerySegmentedAsync(new TableQuery<ConsolidateEntity>(), null);
                            IEnumerable<ConsolidateEntity> existConsolidated = allConsolidated.Where(x => x.idWorker == watchesAuxiliar[i].idWorker);
                            if (existConsolidated == null || existConsolidated.Count() == 0)
                            {
                                ConsolidateEntity consolidated = new ConsolidateEntity
                                {
                                    idWorker = watchesAuxiliar[i].idWorker,
                                    date = DateTime.Today,
                                    minutesWorked = (int)totalMinutes,
                                    ETag = "*",
                                    PartitionKey = "ConsolidatedRegisters",
                                    RowKey = watchesAuxiliar[i].RowKey
                                };
                                TableOperation addConsolidatedOperation = TableOperation.Insert(consolidated);
                                await consolidateTable.ExecuteAsync(addConsolidatedOperation);
                                totalAdded++;
                            }
                            else
                            {
                                TableOperation findOp = TableOperation.Retrieve<ConsolidateEntity>("ConsolidatedRegisters", existConsolidated.First().RowKey);
                                TableResult findRes = await consolidateTable.ExecuteAsync(findOp);
                                ConsolidateEntity consolidatedEntity = (ConsolidateEntity)findRes.Result;
                                consolidatedEntity.date = existConsolidated.First().date;
                                consolidatedEntity.minutesWorked += (int)totalMinutes;
                                TableOperation addConsolidatedOperation = TableOperation.Replace(consolidatedEntity);
                                await consolidateTable.ExecuteAsync(addConsolidatedOperation);
                                totalUpdated++;
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    string errorMessage = error.Message;
                    throw;
                }
            }
            log.LogInformation($"Well done!. \n  New data saved was: {totalAdded} \n Data updated was: {totalUpdated}.");

        }

        private static async Task ChangeConsolidateStatus(string id, CloudTable watchTable)
        {
            TableOperation findOperation = TableOperation.Retrieve<WatchEntity>("RegisterTable", id);
            TableResult findResult = await watchTable.ExecuteAsync(findOperation);
            WatchEntity timeEntity = (WatchEntity)findResult.Result;
            timeEntity.isConsolidate = true;
            TableOperation addOperation = TableOperation.Replace(timeEntity);
            await watchTable.ExecuteAsync(addOperation);
        }
    }
}
