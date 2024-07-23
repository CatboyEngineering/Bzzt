using CatboyEngineering.Bzzt.Models;
using CatboyEngineering.Bzzt.Models.StatusEffects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CatboyEngineering.Bzzt
{
    // Borrowed from https://github.com/Kouzukii/ffxiv-deathrecap/blob/master/Events/CombatEventCapture.cs
    public class StatusEffectListener : IDisposable
    {
        public Plugin Plugin { get; set; }
        public uint PlayerID { get; set; }

        private delegate void ProcessPacketEffectResultDelegate(uint targetId, IntPtr actionIntegrityData, bool isReplay);

        [Signature("48 8B C4 44 88 40 18 89 48 08", DetourName = nameof(ProcessPacketEffectResultDetour))]
        private readonly Hook<ProcessPacketEffectResultDelegate> processPacketEffectResultHook = null!;

        public StatusEffectListener(Plugin plugin)
        {
            Plugin = plugin;

            Plugin.ClientState.Login += Initialize;
        }

        private void Initialize()
        {
            PlayerID = Plugin.ClientState.LocalPlayer.EntityId;

            Plugin.GameInteropProvider.InitializeFromAttributes(this);

            processPacketEffectResultHook.Enable();
        }

        private void onStatusReceived(StatusEffect effect)
        {
            Plugin.Logger.Debug($"User gained status {effect.Id}:{effect.Status}");

            foreach(var trigger in Plugin.Configuration.SavedTriggers)
            {
                if(trigger.TriggerType == TriggerType.STATUS_RECEIVED)
                {
                    if (((int)trigger.TriggerValue) == effect.Id)
                    {
                        Plugin.Logger.Debug($"Dispatching pattern {trigger.PatternName}");

                        // Run the action
                        // TODO: Add safety checks: Is the user allowing commands, is a command running, which toy should this be sent to
                        var command = Plugin.Configuration.SavedPatterns.Find(sp => sp.Name.Equals(trigger.PatternName));

                        if (Plugin.ToyController.ConnectedToys.Count > 0)
                        {
                            var toy = Plugin.ToyController.ConnectedToys.First();

                            _ = Plugin.ToyController.IssueCommand(toy, command);
                        }

                        break;
                    }
                }
            }
        }

        private unsafe void ProcessPacketEffectResultDetour(uint targetId, IntPtr actionIntegrityData, bool isReplay)
        {
            processPacketEffectResultHook.Original(targetId, actionIntegrityData, isReplay);

            try
            {
                var message = (AddStatusEffect*)actionIntegrityData;

                if (targetId != PlayerID)
                {
                    return;
                }

                var effects = (StatusEffectAddEntry*)message->Effects;
                var effectCount = Math.Min(message->EffectCount, 4u);

                for (uint j = 0; j < effectCount; j++)
                {
                    var effect = effects[j];
                    var effectId = effect.EffectId;

                    if (effectId <= 0)
                    {
                        continue;
                    }

                    if (effect.Duration < 0)
                    {
                        continue;
                    }

                    var source = Plugin.ObjectTable.SearchById(effect.SourceActorId)?.Name.TextValue;
                    var status = Plugin.DataManager.Excel.GetSheet<Status>()?.GetRow(effectId);

                    var statusEffect = new StatusEffect
                    {
                        Id = effectId,
                        StackCount = effect.StackCount <= status?.MaxStacks ? effect.StackCount : 0u,
                        Status = status?.Name.RawString.Demangle(),
                        Description = status?.Description.DisplayedText().Demangle(),
                        Source = source,
                        Duration = effect.Duration
                    };

                    onStatusReceived(statusEffect);
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.Error(e, "Caught unexpected exception");
            }
        }

        public void Dispose()
        {
            processPacketEffectResultHook.Dispose();
        }
    }
}
