using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using watchStewar.Common.Models;
using watchStewar.Functions.Entities;

namespace watchStewar.Tests.Helpers
{
    public class TestFactory
    {
        public static WatchEntity GetWatchEntity()
        {
            return new WatchEntity
            {
                ETag = "*",
                PartitionKey = "RegisterTable",
                RowKey = Guid.NewGuid().ToString(),
                idWorker = 1,
                register = DateTime.UtcNow,
                type = 0,
                isConsolidate = false
            };
        }

        public static List<WatchEntity> GetWatchesEntities()
        {
            return new List<WatchEntity>();
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid watchId, Watch watchRequest)
        {
            string request = JsonConvert.SerializeObject(watchRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{watchId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid watchId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{watchId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Watch watchRequest)
        {
            string request = JsonConvert.SerializeObject(watchRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Watch GetWatchRequest()
        {
            return new Watch
            {
                idWorker = 1,
                register = DateTime.UtcNow,
                type = 0,
                isConsolidate = false
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
