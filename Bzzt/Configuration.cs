﻿using CatboyEngineering.Bzzt.Models.Toy;
using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace CatboyEngineering.Bzzt
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public string IntifaceServerAddress { get; set; } = "ws://localhost:12345";
        public List<StoredShellCommand> SavedPatterns { get; set; } = new List<StoredShellCommand>();

        public int Version { get; set; } = 0;

        [NonSerialized]
        private readonly int CurrentVersion = 0;

        [NonSerialized]
        public readonly string CaptchaToken = "DalamudClient";

        [NonSerialized]
        private IDalamudPluginInterface PluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;

            PerformVersionUpdates();
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }

        public Configuration Clone()
        {
            var clone = new Configuration();

            clone.IntifaceServerAddress = this.IntifaceServerAddress;
            clone.SavedPatterns = this.SavedPatterns;

            return clone;
        }

        public void Import(Configuration configuration)
        {
            this.IntifaceServerAddress = configuration.IntifaceServerAddress;
            this.SavedPatterns = configuration.SavedPatterns;
        }

        private void PerformVersionUpdates()
        {

        }
    }
}
