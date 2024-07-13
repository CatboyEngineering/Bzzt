using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt.Models.StatusEffects
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AddStatusEffect
    {
        public uint Unknown1;

        public uint RelatedActionSequence;

        public uint ActorId;

        public uint CurrentHp;

        public uint MaxHp;

        public ushort CurrentMp;

        public ushort Unknown3;

        public byte DamageShield;

        public byte EffectCount;

        public ushort Unknown6;

        public unsafe fixed byte Effects[64];
    }
}
