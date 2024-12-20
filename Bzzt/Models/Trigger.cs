﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt.Models
{
    public class Trigger
    {
        public Guid TriggerID { get; set; }
        public TriggerType TriggerType { get; set; }
        public XIVStatus TriggerValue { get; set; }
        public string PatternName { get; set; }

        public Trigger()
        {
            TriggerID = Guid.NewGuid();
            TriggerType = TriggerType.STATUS_RECEIVED;
            TriggerValue = XIVStatus.VULNERABILITY_UP;
            PatternName = string.Empty;
        }
    }
}
