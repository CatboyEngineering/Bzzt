using CatboyEngineering.Bzzt.Models.Toy;
using CatboyEngineering.Bzzt.Windows.States.Models;
using System.Collections.Generic;

namespace CatboyEngineering.Bzzt.Windows.States
{
    public class TrigerWindowState
    {
        public Plugin Plugin { get; set; }
        public List<TriggerStateItem> Triggers { get; set; }

        public TrigerWindowState(Plugin plugin)
        {
            Plugin = plugin;
            Triggers = new List<TriggerStateItem>();

            SetDefauts();
        }

        public void SetDefauts()
        {
            ResetBuffers();
        }

        public void ResetBuffers()
        {

        }
    }
}
