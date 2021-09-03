using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using watchStewar.Common.Models;
using watchStewar.Functions.Entities;
using watchStewar.Functions.Functions;
using watchStewar.Tests.Helpers;
using Xunit;

namespace watchStewar.Tests.Test
{
    public class WatchAPITest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateRegister_Should_Return_200()
        {

            //arrange
            MockCloudTableWatches mockTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Watch watchRequest = TestFactory.GetWatchRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(watchRequest);

            //act
            IActionResult response = await WatchAPI.CreateRegister(request, mockTable, logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void UpdateRegister_Should_Return_200()
        {

            //arrange
            MockCloudTableWatches mockTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Watch watchRequest = TestFactory.GetWatchRequest();
            Guid watchId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(watchRequest);

            //act
            IActionResult response = await WatchAPI.UpdateRegisterById(request, mockTable, watchId.ToString(), logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void DeleteRegister_Should_Return_200()
        {

            //arrange
            MockCloudTableWatches mockTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Watch watchRequest = TestFactory.GetWatchRequest();
            WatchEntity watchEntity = TestFactory.GetWatchEntity();
            Guid watchId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(watchRequest);

            //act
            IActionResult response = await WatchAPI.DeleteRegisterById(request, watchEntity, mockTable, watchId.ToString(), logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllRegisters_Should_Return_200()
        {
            //arrange
            MockCloudTableWatches mockTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Watch watchRequest = TestFactory.GetWatchRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(watchRequest);

            //act
            IActionResult response = await WatchAPI.GetAllRegisters(request, mockTable, logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void GetRegisterById_Should_Return_200()
        {

            //arrange
            MockCloudTableWatches mockTable = new MockCloudTableWatches(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            WatchEntity watchEntity = TestFactory.GetWatchEntity();
            Guid watchId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            //act
            IActionResult response = WatchAPI.GetRegisterById(request, watchEntity, watchId.ToString(), logger);

            //assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
