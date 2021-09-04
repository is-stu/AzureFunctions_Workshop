using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using watchStewar.Functions.Functions;
using watchStewar.Tests.Helpers;
using Xunit;

namespace watchStewar.Tests.Test
{
    public class ConsolidateAPITest
    {
        [Fact]
        public async Task GetConsolidateByDate_Should_Return_200()
        {

            //arrange
            MockCloudTableConsolidates mockTable = new MockCloudTableConsolidates(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ILogger logger = TestFactory.CreateLogger();
            string date = "2021/09/04";
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(date);

            //act
            IActionResult response = await ConsolidateAPI.GetConsolidateByDate(request, mockTable, date, logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
