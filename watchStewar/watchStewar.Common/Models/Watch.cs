using System;

namespace watchStewar.Common.Models
{
    public class Watch
    {
        public int idWorker { get; set; }

        public DateTime register { get; set; }

        public byte type { get; set; }

        public bool isConsolidate { get; set; }
    }
}
