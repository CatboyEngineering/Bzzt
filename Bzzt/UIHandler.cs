using CatboyEngineering.Bzzt.Windows;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using System;

namespace CatboyEngineering.Bzzt
{
    public class UIHandler : IDisposable
    {
        public WindowSystem WindowSystem = new("Bzzt");

        public Plugin Plugin { get; init; }
        private IDalamudPluginInterface PluginInterface { get; init; }

        public ConfigWindow ConfigWindow { get; init; }
        public MainWindow MainWindow { get; init; }
        public PatternBuilderWindow PatternBuilderWindow { get; set; }
        public TriggerWindow TriggerWindow { get; set; }

        public UIHandler(Plugin plugin, IDalamudPluginInterface pluginInterface)
        {
            this.Plugin = plugin;
            this.PluginInterface = pluginInterface;

            ConfigWindow = new ConfigWindow(this.Plugin);
            MainWindow = new MainWindow(this.Plugin);
            PatternBuilderWindow = new PatternBuilderWindow(this.Plugin);
            TriggerWindow = new TriggerWindow(this.Plugin);

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);
            WindowSystem.AddWindow(PatternBuilderWindow);
            WindowSystem.AddWindow(TriggerWindow);

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            this.PluginInterface.UiBuilder.OpenMainUi += OpenMainWindow;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void OpenMainWindow()
        {
            MainWindow.IsOpen = true;
        }

        public void OpenTriggerWindow()
        {
            TriggerWindow.IsOpen = true;
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();
            MainWindow.Dispose();
            PatternBuilderWindow.Dispose();
            TriggerWindow.Dispose();
        }
    }
}