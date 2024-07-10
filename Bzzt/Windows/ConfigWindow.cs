using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace CatboyEngineering.Bzzt.Windows
{
    public class ConfigWindow : Window, IDisposable
    {
        private Plugin plugin;
        private Configuration Configuration;
        private Configuration WorkingCopy;
        private readonly Regex IntifacePath = new("^(wss?)(:\\/\\/)[\\w\\d]+[:.\\w\\d/]+$");

        public ConfigWindow(Plugin plugin) : base("Bzzt Configuration", ImGuiWindowFlags.NoResize)
        {
            this.Configuration = plugin.Configuration;
            this.plugin = plugin;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.WorkingCopy = Configuration.Clone();
        }

        public override void Draw()
        {   
            ImGui.SetNextWindowSize(new Vector2(400, 425), ImGuiCond.Always);

            if (ImGui.Begin("Bzzt Configuration"))
            {
                DrawUIWindowBody();
                DrawUIWindowFooter();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            DrawUIIntifaceServerTabItem();
        }

        private void DrawUIIntifaceServerTabItem()
        {
            var intifaceServer = this.WorkingCopy.IntifaceServerAddress;

            ImGui.Text("Intiface Address");

            if (ImGui.InputText("##IntifaceAddress", ref intifaceServer, 64))
            {
                if (IntifacePath.IsMatch(intifaceServer))
                {
                    this.WorkingCopy.IntifaceServerAddress = intifaceServer;
                }
            }
        }

        private void DrawUIWindowFooter()
        {
            if (ImGui.Button("Save and Close"))
            {
                Configuration.Import(WorkingCopy);
                Configuration.Save();
                this.IsOpen = false;
            }
        }
    }
}
