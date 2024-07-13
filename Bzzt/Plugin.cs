using CatboyEngineering.Bzzt.Toy;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using System.Reflection;

namespace CatboyEngineering.Bzzt
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService] internal static IPluginLog Logger { get; private set; } = null!;
        [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
        [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
        [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
        [PluginService] internal static IClientState ClientState { get; private set; } = null!;

        public Configuration Configuration { get; }
        public bool IsDev { get; set; }

        public CommandHandler CommandHandler { get; }
        public UIHandler UIHandler { get; }
        public ToyController ToyController { get; }
        public StatusEffectListener StatusEffectListener { get; }

        public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        public string Name => "Bzzt";

        public Plugin()
        {
            this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(PluginInterface);

            IsDev = PluginInterface.IsDev;

            CommandHandler = new CommandHandler(this, CommandManager);
            UIHandler = new UIHandler(this, PluginInterface);

            ToyController = new ToyController(this);
            StatusEffectListener = new StatusEffectListener(this);
        }

        public void Dispose()
        {
            CommandHandler.Dispose();
            UIHandler.Dispose();
            ToyController.Dispose();
            StatusEffectListener.Dispose();
        }
    }
}