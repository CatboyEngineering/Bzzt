using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt.Models.StatusEffects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StatusEffectAddEntry
    {
        public byte EffectIndex;

        public byte Unknown1;

        public ushort EffectId;

        public ushort StackCount;

        public ushort Unknown3;

        public float Duration;

        public uint SourceActorId;
    }
}
