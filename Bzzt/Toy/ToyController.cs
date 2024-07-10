using Buttplug.Client;
using Buttplug.Client.Connectors.WebsocketConnector;
using Buttplug.Core.Messages;
using CatboyEngineering.Bzzt.Models.Toy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Buttplug.Core.Messages.ScalarCmd;

namespace CatboyEngineering.Bzzt.Toy
{
    public class ToyController : IDisposable
    {
        public Plugin Plugin { get; init; }
        public ButtplugWebsocketConnector Connector { get; private set; }
        public ButtplugClient Client { get; private set; }
        public bool StopRequested { get; set; }
        public List<ToyProperties> ConnectedToys { get; set; }

        public ToyController(Plugin plugin)
        {
            Plugin = plugin;
            StopRequested = false;
            ConnectedToys = new List<ToyProperties>();
        }

        public bool IsConnected()
        {
            if(Client != null)
            {
                return Client.Connected;
            }

            return false;
        }

        public async Task Connect()
        {
            await Task.Run(() =>
            {
                Connector = new ButtplugWebsocketConnector(new Uri($"{Plugin.Configuration.IntifaceServerAddress}"));
                Client = new ButtplugClient("Bzzt Client");
            });

            Client.DeviceAdded += DeviceAdded;
            Client.DeviceRemoved += DeviceRemoved;

            try
            {
                await Client.ConnectAsync(Connector);
                await Scan();
            }
            catch { }
        }

        private void DeviceAdded(object? sender, DeviceAddedEventArgs args)
        {
            AddConnectedToy(args.Device);
        }

        private void DeviceRemoved(object? sender, DeviceRemovedEventArgs args)
        {
            RemoveConnectedToy(args.Device);
        }

        public async Task Scan()
        {
            if (Client.Connected)
            {
                await Client.StartScanningAsync();
                await Task.Delay(3000);
                await Client.StopScanningAsync();

                ConnectedToys.Clear();

                foreach (var toy in Client.Devices)
                {
                    AddConnectedToy(toy);
                }
            }
        }

        private void AddConnectedToy(ButtplugClientDevice device)
        {
            ConnectedToys.Add(new ToyProperties(device));
        }

        private void RemoveConnectedToy(ButtplugClientDevice device)
        {
            // Is this potentially dangerous? Will the library shift the indexes?
            ConnectedToys.RemoveAll(ct => ct.Index == device.Index);
        }

        public async Task Disconnect()
        {
            if (Client.Connected)
            {
                await Client.DisconnectAsync();
            }
        }

        public void StopAllDevices()
        {
            if (Client.Connected)
            {
                foreach (var device in Client.Devices)
                {
                    _ = device.Stop();
                }
            }
        }

        public async Task IssueCommand(ToyProperties toy, StoredShellCommand command)
        {
            if (Client.Connected)
            {
                Plugin.Logger.Info("Translating action to Intiface.");

                var device = Client.Devices[toy.Index];
                var vibrateM2AdjustmentMS = 100;

                foreach (var pattern in command.Instructions)
                {
                    if (StopRequested)
                    {
                        StopRequested = false;
                        break;
                    }

                    if (pattern.IsValid())
                    {
                        // Begin fix for vibration toys - a small blip was found between steps, likely caused by motor taking too long to spin up.
                        var currentIndex = command.Instructions.IndexOf(pattern);

                        if (command.Instructions.Count >= currentIndex + 2)
                        {
                            var nextInstruction = command.Instructions[currentIndex + 1];

                            if (pattern.PatternType == PatternType.VIBRATE && nextInstruction.PatternType == PatternType.VIBRATE)
                            {
                                // m0
                                if (pattern.VibrateIntensity[0] == 0d && nextInstruction.VibrateIntensity[0] > 0d)
                                {
                                    _ = Task.Delay(pattern.Duration - vibrateM2AdjustmentMS).ContinueWith(async t =>
                                    {
                                        await VibrateAsync(device, toy, new double[] { nextInstruction.VibrateIntensity[0], pattern.VibrateIntensity[1] });
                                        await Task.Delay(vibrateM2AdjustmentMS);
                                    });
                                }
                                // m1
                                else if (pattern.VibrateIntensity[1] == 0d && nextInstruction.VibrateIntensity[1] > 0d)
                                {
                                    _ = Task.Delay(pattern.Duration - vibrateM2AdjustmentMS).ContinueWith(async t =>
                                    {
                                        await VibrateAsync(device, toy, new double[] { pattern.VibrateIntensity[0], nextInstruction.VibrateIntensity[1] });
                                        await Task.Delay(vibrateM2AdjustmentMS);
                                    });
                                }
                            }
                        }
                        // End vibration fix.

                        try
                        {
                            switch (pattern.PatternType)
                            {
                                case PatternType.CONSTRICT:
                                    await device.ScalarAsync(new ScalarSubcommand(0, pattern.ConstrictAmount.Value, ActuatorType.Constrict));
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.INFLATE:
                                    await device.ScalarAsync(new ScalarSubcommand(0, pattern.InflateAmount.Value, ActuatorType.Inflate));
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.LINEAR:
                                    await device.LinearAsync((uint)pattern.Duration, pattern.LinearPosition.Value);
                                    break;
                                case PatternType.OSCILLATE:
                                    await device.OscillateAsync(pattern.OscillateIntensity);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.ROTATE:
                                    await device.RotateAsync(pattern.RotateSpeed.Value, pattern.RotateClockwise.Value);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                case PatternType.VIBRATE:
                                    await VibrateAsync(device, toy, pattern.VibrateIntensity);
                                    await Task.Delay(pattern.Duration);
                                    break;
                                default:
                                    await device.Stop();
                                    await Task.Delay(pattern.Duration);

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.Logger.Warning(ex, "Bzzt action error");
                        }
                    }
                    else
                    {
                        Plugin.Logger.Warning("Received an invalid action!");
                    }
                }

                await device.Stop();
            }
        }

        public void Dispose()
        {
            if (Client != null)
            {
                foreach (var device in Client.Devices)
                {
                    _ = device.Stop();
                }

                if (Client.Connected)
                {
                    Client.DisconnectAsync();
                }

                Client.Dispose();
            }

            if (Connector != null)
            {
                Connector.Dispose();
            }
        }

        private async Task VibrateAsync(ButtplugClientDevice device, ToyProperties toy, double[] vibrateInstructions)
        {
            if (toy.Vibrate == 2)
            {
                await device.VibrateAsync(vibrateInstructions);
            }
            else if (toy.Vibrate > 0)
            {
                await device.VibrateAsync(vibrateInstructions[0]);
            }
        }
    }
}