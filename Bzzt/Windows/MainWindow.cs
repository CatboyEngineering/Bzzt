﻿using CatboyEngineering.Bzzt.Windows.States;
using CatboyEngineering.Bzzt.Windows.Utilities;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using ImGuiNET;
using System;
using System.Numerics;

namespace CatboyEngineering.Bzzt.Windows
{
    public class MainWindow : Window, IDisposable
    {
        private Plugin Plugin;

        public MainWindowState State { get; init; }

        public MainWindow(Plugin plugin) : base($"Bzzt v{Plugin.Version}", ImGuiWindowFlags.NoResize)
        {
            this.Plugin = plugin;
            State = new MainWindowState(plugin);
        }

        public override void OnOpen()
        {
            base.OnOpen();

            if (!Plugin.Configuration.IntifaceServerAddress.IsNullOrEmpty() && !Plugin.ToyController.IsConnected())
            {
                var task = Plugin.ToyController.Connect();
                _ = MainWindowUtilities.HandleWithIndicator(State, task);
            }
        }

        public override void Draw()
        {
            ImGui.SetNextWindowSize(new Vector2(410, 400), ImGuiCond.Always);

            if (ImGui.Begin($"Bzzt v{Plugin.Version}"))
            {
                DrawUIWindowBody();
            }

            ImGui.End();
        }

        public void Dispose() { }

        private void DrawUIWindowBody()
        {
            if (!Plugin.Configuration.IntifaceServerAddress.IsNullOrEmpty())
            {
                if (State.isRequestInFlight)
                {
                    ImGui.Text("Connecting to Intiface...");
                }
                else if(!Plugin.ToyController.IsConnected())
                {
                    ImGui.Text("Please ensure Intiface is running");
                    ImGui.Text("before reconnecting.");
                    DrawUIReconnectButton();
                }
                else
                {
                    DrawUIWindowLoggedInHomepage();
                }
            }
            else
            {
                DrawUIWindowNeedsIntiface();
            }
        }

        private void DrawUIWindowNeedsIntiface()
        {
            var text = "Please add your Intiface address.";

            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            DrawUIErrorText(text);

            var settingsTextWidth = ImGui.CalcTextSize("Settings").X;
            ImGui.SetCursorPosX((windowWidth - settingsTextWidth) * 0.5f);
            DrawUISettingsButton();
        }

        private void DrawUIWindowLoggedInHomepage()
        {
            var welcomeText = $"Welcome to Bzzt!";

            DrawUICenteredText(welcomeText);
            DrawUITriggerWindowButton();

            ImGui.SameLine();

            if (ImGui.Button("Pattern Builder"))
            {
                Plugin.UIHandler.PatternBuilderWindow.IsOpen = true;
            }

            DrawUIConnectedToys();
            DrawUIDisconnectButton();
        }

        private void DrawUIConnectedToys()
        {
            ImGui.Spacing();

            if (Plugin.ToyController.Client != null)
            {
                if (Plugin.ToyController.Client.Connected)
                {
                    ImGui.Text("Intiface Connected!");

                    var width = ImGui.GetWindowWidth();
                    ImGui.BeginChild("IntifaceWindow", new Vector2(width - 15, 75), true);

                    if (Plugin.ToyController.ConnectedToys.Count > 0)
                    {
                        foreach (var toy in Plugin.ToyController.ConnectedToys)
                        {
                            ImGui.BulletText("Connected to " + toy.DisplayName);
                        }
                    }
                    else
                    {
                        ImGui.Text("No Connected Devices");
                    }

                    if (ImGui.Button("Re-scan"))
                    {
                        _ = Plugin.ToyController.Scan();
                    }

                    ImGui.EndChild();
                }
                else
                {
                    ImGui.Text("Intiface not connected");
                    ImGui.SameLine();

                    if (ImGui.Button("Retry"))
                    {
                        _ = Plugin.ToyController.Connect();
                    }
                }
            }
            else
            {
                ImGui.Text("Intiface connecting...");
            }
        }

        private void DrawUIReconnectButton()
        {
            if (!State.isRequestInFlight)
            {
                if (ImGui.Button("Reconnect"))
                {
                    var task = Plugin.ToyController.Connect();
                    _ = MainWindowUtilities.HandleWithIndicator(State, task);
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button("Connecting...");
                ImGui.EndDisabled();
            }
        }

        private void DrawUIDisconnectButton()
        {
            // Put logout button in right corner.
            var buttonTextWidth = ImGui.CalcTextSize("Disconnect").X;
            var windowWidth = ImGui.GetWindowSize().X;
            ImGui.SetCursorPosX((windowWidth - buttonTextWidth) * 0.90f);

            if (ImGui.Button("Disconnect"))
            {
                DisconnectAll();
            }
        }

        private void DrawUISettingsButton()
        {
            if (ImGui.Button("Settings"))
            {
                Plugin.UIHandler.DrawConfigUI();
            }
        }

        private void DrawUITriggerWindowButton()
        {
            if (ImGui.Button("Triggers"))
            {
                Plugin.UIHandler.OpenTriggerWindow();
            }
        }

        private void DrawUIErrorText(string text)
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), text);
        }

        private void DrawUICenteredText(string text)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((windowWidth - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        private void DisconnectAll()
        {
            _ = Plugin.ToyController.Disconnect();
        }
    }
}