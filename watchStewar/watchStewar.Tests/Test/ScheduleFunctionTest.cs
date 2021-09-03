using System;
using watchStewar.Functions.Functions;
using watchStewar.Tests.Helpers;
using Xunit;

namespace watchStewar.Tests.Test
{
    public class ScheduleFunctionTest
    {
        [Fact]
        public void ScheduleFunction_Should_Log_Message()
        {

            //arrange
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            MockCloudTableWatches watchTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableWatches consolidateTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));


            //act
            ScheduleFunction.Run(null, watchTable, consolidateTable, logger);
            string message = logger.Logs[0];

            //assert
            Assert.Contains("Consolidating data in table consolidate", message);
        }
    }
}
