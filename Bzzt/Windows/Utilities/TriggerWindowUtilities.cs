using CatboyEngineering.Bzzt.Models;
using CatboyEngineering.Bzzt.Models.Toy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CatboyEngineering.Bzzt.Windows.Utilities
{
    public class TriggerWindowUtilities
    {
        public static void CreateNewTrigger(TriggerWindow window)
        {
            window.Plugin.Configuration.SavedTriggers.Add(new Trigger());
        }
    }
}
