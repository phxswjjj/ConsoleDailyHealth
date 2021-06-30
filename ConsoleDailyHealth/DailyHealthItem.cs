using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleDailyHealth
{
    public class DailyHealthItem
    {
        public int itemId { get; set; }
        public string description { get; set; }
        public bool? selected { get; set; }
        public bool? isNeedTrack { get; set; }
    }
}
