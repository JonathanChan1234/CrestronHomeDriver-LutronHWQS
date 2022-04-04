using System;
using System.Collections.Generic;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp;
using LutronMotorDevice;
using LutronSwitchingDevice;

namespace LutronHWQSGateway
{
    public class LutronHWQSGatewayProtocol : AGatewayProtocol
    {
        #region Fields
        private bool _isLoginDone = false;
        private bool _monitoringCommandSent = false;
        private CTimer _sendLoginCommandTimer;
        private CTimer _sendPasswordCommandTimer;

        private readonly Dictionary<int, ALutronSwitchingDevice> _switches =
            new Dictionary<int, ALutronSwitchingDevice>();

        private readonly Dictionary<int, ALutronMotorDevice> _motors =
                   new Dictionary<int, ALutronMotorDevice>();

        #endregion

        #region Initialization

        public LutronHWQSGatewayProtocol(ISerialTransport transport, byte id)
            : base(transport, id)
        {
            ValidateResponse = GatewayValidateResponse;
            ALutronSwitchingDevice[] _switchList = new ALutronSwitchingDevice[] {
                new ALutronSwitchingDevice(40, "BAL1-1", SwitchLoadType.ExhaustFan),
                new ALutronSwitchingDevice(41, "BAL1-2", SwitchLoadType.Hood),
                new ALutronSwitchingDevice(42, "BAL2-1", SwitchLoadType.Light),
                new ALutronSwitchingDevice(43, "BAL2-2", SwitchLoadType.WaterHeater)
            };
            ALutronMotorDevice[] _motorList = new ALutronMotorDevice[]
            {
                new ALutronMotorDevice(46, "Motor-46"),
                new ALutronMotorDevice(47, "Motor-47"),
                new ALutronMotorDevice(44, "Motor-48"),
                new ALutronMotorDevice(45, "Motor-49")
            };
            foreach (var _switch in _switchList)
            {
                _switch.SetConnectionStatus(true);
                AddSwitchPairedDevice(_switch);
                _switch.PowerStateChangedEvent += PowerStateChangeEventHandler;
            }
            foreach (var motor in _motorList)
            {
                motor.SetConnectionStatus(true);
                AddMotorPairedDevice(motor);
                motor.MotorActionEvent += MotorActionEventHandler;
            }
            _sendLoginCommandTimer = new CTimer(SendLoginCommand, Timeout.Infinite);
            _sendPasswordCommandTimer = new CTimer(SendPasswordCommand, Timeout.Infinite);
        }

        private void MotorActionEventHandler(object sender, Crestron.RAD.Common.Events.ValueEventArgs<MotorAction> e)
        {
            try
            {
                ALutronMotorDevice device = (ALutronMotorDevice)sender;
                int id = device.Id;
                Log($"Device sent to Gateway: Button pressed event handler: {e.Value}, Received from {id}");
                if (_isLoginDone)
                    SendCommand(LutronCommand.MotorControlCommmand(id, e.Value));
            }
            catch (Exception error)
            {
                if (EnableLogging)
                {
                    Log($"Button Pressed Event in Gateway Protocol");
                    Log(error.Message);
                }
            }
        }

        private void PowerStateChangeEventHandler(object sender, Crestron.RAD.Common.Events.ValueEventArgs<bool> e)
        {
            try
            {
                ALutronSwitchingDevice device = (ALutronSwitchingDevice)sender;
                int id = device.Id;
                Log($"Device sent to Gateway: Button pressed event handler: {e.Value}, Received from {id}");
                Log($"IsLoginDone: {_isLoginDone}");
                if (_isLoginDone)
                    SendCommand(LutronCommand.ZoneControlCommand(id, e.Value ? 100 : 0));
            }
            catch (Exception error)
            {
                if (EnableLogging)
                {
                    Log($"Button Pressed Event in Gateway Protocol");
                    Log(error.Message);
                }
            }
        }
        #endregion

        #region Base Members

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            LutronRXEventArgs args = LutronRxParser.Parse(validatedData.Data);
            switch (args.Type)
            {
                case EventType.Login:
                    Log("Sending Login command");
                    _monitoringCommandSent = false;
                    _sendLoginCommandTimer.Reset(5000);
                    break;
                case EventType.Password:
                    Log("Sending Password command");
                    _monitoringCommandSent = false;
                    _sendPasswordCommandTimer.Reset(5000);
                    break;
                case EventType.Prompt:
                    _isLoginDone = true;
                    if (!_monitoringCommandSent)
                    {
                        Log("Sending Monitoring command");
                        SendCommand(LutronCommand.MonitoringCommand);
                    }
                    break;
                case EventType.Monitoring:
                    _monitoringCommandSent = true;
                    break;
                case EventType.Zone:
                    double? brightness = args.Brightness;
                    int? integrationId = args.Id;
                    if (brightness == null || integrationId == null) return;
                    ALutronSwitchingDevice device;
                    _switches.TryGetValue((int)integrationId, out device);
                    if (device == null) return;
                    Log($"Feedback recevied from Gateway: {(int)brightness} (Integration ID: {device.Id}");
                    device.SetPower(brightness > 0);
                    break;
                case EventType.Error:
                    if (EnableLogging) Log(args.Message);
                    break;
            }
        }

        protected override void ConnectionChangedEvent(bool connection)
        {
            base.ConnectionChangedEvent(connection);

            foreach (var _pairedDevice in _switches.Values)
            {
                _pairedDevice.SetConnectionStatus(true);
            }
            foreach (var _pairedDevice in _motors.Values)
            {
                _pairedDevice.SetConnectionStatus(true);
            }
        }

        #endregion

        #region Public Members

        public void Connect()
        {
            if (EnableLogging) Log("Connected to Lutron Telnet Server");
            _monitoringCommandSent = false;
        }

        public void Disconnect()
        {
            if (EnableLogging) Log("Disconnected from Lutron Telnet Server");
        }

        #endregion

        #region Private Members

        private void AddSwitchPairedDevice(ALutronSwitchingDevice pairedDevice)
        {
            // Set connection status on device if the device created after the gateway is online.
            // pairedDevice.SetConnectionStatus(IsConnected);

            if (_switches.ContainsKey(pairedDevice.Id)) return;
            var pairedDeviceInformation = new GatewayPairedDeviceInformation(pairedDevice.Id.ToString(),
                pairedDevice.Name, pairedDevice.Description, pairedDevice.Manufacturer, pairedDevice.Model,
               pairedDevice.DeviceType,
                pairedDevice.DeviceSubtype);
            AddPairedDevice(pairedDeviceInformation, pairedDevice);
            _switches.Add(pairedDevice.Id, pairedDevice);
        }
        private void AddMotorPairedDevice(ALutronMotorDevice pairedDevice)
        {
            // Set connection status on device if the device created after the gateway is online.
            // pairedDevice.SetConnectionStatus(IsConnected);

            if (_motors.ContainsKey(pairedDevice.Id)) return;
            var pairedDeviceInformation = new GatewayPairedDeviceInformation(pairedDevice.Id.ToString(),
                pairedDevice.Name, pairedDevice.Description, pairedDevice.Manufacturer, pairedDevice.Model,
               pairedDevice.DeviceType,
                pairedDevice.DeviceSubtype);
            AddPairedDevice(pairedDeviceInformation, pairedDevice);
            _motors.Add(pairedDevice.Id, pairedDevice);
        }

        private void UpdateSamplePairedDevice(ALutronSwitchingDevice pairedDevice)
        {
            pairedDevice.SetConnectionStatus(IsConnected);
            if (_switches.ContainsKey(pairedDevice.Id))
            {
                var pairedDeviceInformation = new GatewayPairedDeviceInformation(pairedDevice.Id.ToString(),
                    pairedDevice.Name, pairedDevice.Description, pairedDevice.Manufacturer, pairedDevice.Model,
                    pairedDevice.DeviceType,
                    pairedDevice.DeviceSubtype);

                UpdatePairedDevice(pairedDevice.Id.ToString(), pairedDeviceInformation);
            }
        }

        private void RemovePairedDevice(ALutronSwitchingDevice pairedDevice)
        {
            if (_switches.ContainsKey(pairedDevice.Id))
            {
                RemovePairedDevice(pairedDevice.Id.ToString());
            }
        }

        private ValidatedRxData GatewayValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            return new ValidatedRxData(true, response);
        }
        #endregion

        private void SendLoginCommand(object command)
        {
            SendCommand(LutronCommand.LoginCommand);
        }

        private void SendPasswordCommand(object command)
        {
            SendCommand(LutronCommand.PasswordCommand);
        }
        private void SendMonitoringCommand(object command)
        {
            SendCommand(LutronCommand.MonitoringCommand);
        }

        public override void Dispose()
        {
            _sendLoginCommandTimer.Stop();
            _sendPasswordCommandTimer.Stop();
            try
            {
                foreach (var device in _switches.Values)
                {
                    if (device is IDisposable)
                        ((IDisposable)device).Dispose();
                }
                foreach (var device in _motors.Values)
                {
                    if (device is IDisposable)
                        ((IDisposable)device).Dispose();
                }
                Log("Remove all the devices in the list");
                _switches.Clear();
                _motors.Clear();
            }
            finally
            {
            }

            base.Dispose();
        }
    }
}