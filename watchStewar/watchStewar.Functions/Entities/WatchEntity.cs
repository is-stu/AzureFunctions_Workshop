using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace watchStewar.Functions.Entities
{
    public class WatchEntity : TableEntity
    {
        public int idWorker { get; set; }

        public DateTime register { get; set; }

        public byte type { get; set; }

        public bool isConsolidate { get; set; }
    }
}
