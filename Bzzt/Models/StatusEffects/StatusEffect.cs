using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt.Models.StatusEffects
{
    public struct StatusEffect
    {
        public ushort Id { get; set; }
        public uint StackCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public float Duration { get; set; }
    }
}
