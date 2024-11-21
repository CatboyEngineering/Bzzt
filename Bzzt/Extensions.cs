using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.System.String;
using Lumina.Text;
using Lumina.Text.Payloads;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace CatboyEngineering.Bzzt
{
    public static class Extensions
    {
        public static string DisplayedText(this SeString str)
        {
            return str.Payloads.Aggregate("", (a, p) => p is TextPayload ? a + p.RawString : a);
        }
    }
}