using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using CatboyEngineering.Bzzt.Models;
using CatboyEngineering.Bzzt.Windows.States;
using CatboyEngineering.Bzzt.Windows.Utilities;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace CatboyEngineering.Bzzt.Windows
{
    public class TriggerWindow : Window, IDisposable
    {
        public Plugin Plugin { get; set; }
        public TrigerWindowState State { get; set; }

        public TriggerWindow(Plugin plugin) : base("Bzzt Triggers", ImGuiWindowFlags.NoResize)
        {
            this.Plugin = plugin;
            this.State = new TrigerWindowState(plugin);
        }

        public override void OnClose()
        {
            base.OnClose();
            State.SetDefauts();
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(800, 500), ImGuiCond.Always);

            if (ImGui.Begin("Bzzt Triggers"))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            ImGui.Text("Triggers");
            ImGui.SameLine();

            if (ImGui.Button("+ New Trigger"))
            {
                TriggerWindowUtilities.CreateNewTrigger(this);
            }

            ImGui.SameLine();

            if (ImGui.Button("Save"))
            {
                Plugin.Configuration.Save();
            }

            var width = ImGui.GetWindowWidth();
            ImGui.BeginChild("TriggerList", new Vector2(width - 15, 500), true);

            DrawUITriggerList();

            ImGui.EndChild();
        }

        private void DrawUITriggerList()
        {
            var triggerTypeList = Enum.GetNames(typeof(TriggerType));
            var statusTypeList = Enum.GetNames(typeof(XIVStatus));
            var patternList = Plugin.Configuration.SavedPatterns.Select(sp => sp.Name).ToList();

            foreach (var triggerCopy in new List<Trigger>(Plugin.Configuration.SavedTriggers))
            {
                var triggerTypePreview = triggerCopy.TriggerType.ToString();

                ImGui.SetNextItemWidth(200);

                if (ImGui.BeginCombo($"##selectType{triggerCopy.TriggerID}", triggerTypePreview))
                {
                    for (int i = 0; i < triggerTypeList.Length; i++)
                    {
                        if (ImGui.Selectable(triggerTypeList[i]))
                        {
                            var trigger = Plugin.Configuration.SavedTriggers.Find(st => st.TriggerID == triggerCopy.TriggerID);
                            trigger.TriggerType = Enum.Parse<TriggerType>(triggerTypeList[i]);
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                var triggerValuePreview = triggerCopy.TriggerValue.ToString();

                ImGui.SetNextItemWidth(200);

                if (ImGui.BeginCombo($"##selectValue{triggerCopy.TriggerID}", triggerValuePreview))
                {
                    for (int i = 0; i < statusTypeList.Length; i++)
                    {
                        if (ImGui.Selectable(statusTypeList[i]))
                        {
                            var trigger = Plugin.Configuration.SavedTriggers.Find(st => st.TriggerID == triggerCopy.TriggerID);
                            trigger.TriggerValue = Enum.Parse<XIVStatus>(statusTypeList[i]);
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();
                ImGui.SetNextItemWidth(200);

                var patternPreview = triggerCopy.PatternName;

                if (ImGui.BeginCombo($"##selectPattern{triggerCopy.TriggerID}", patternPreview))
                {
                    for (int i = 0; i < patternList.Count; i++)
                    {
                        if (ImGui.Selectable(patternList[i]))
                        {
                            var trigger = Plugin.Configuration.SavedTriggers.Find(st => st.TriggerID == triggerCopy.TriggerID);
                            trigger.PatternName = patternList[i];
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                if(ImGui.Button($"Delete##{triggerCopy.TriggerID}"))
                {
                    Plugin.Configuration.SavedTriggers.RemoveAll(st => st.TriggerID == triggerCopy.TriggerID);
                }
            }
        }
    }
}
