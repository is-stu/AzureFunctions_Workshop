using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace watchStewar.Functions.Entities
{
    public class ConsolidateEntity : TableEntity
    {
        public int idWorker { get; set; }

        public DateTime date { get; set; }

        public int minutesWorked { get; set; }
    }
}
