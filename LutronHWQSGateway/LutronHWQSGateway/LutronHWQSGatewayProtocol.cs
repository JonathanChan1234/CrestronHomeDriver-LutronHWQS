﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp;
using LutronMotorDevice;
using LutronSwitchingDevice;
using System.Xml;

namespace LutronHWQSGateway
{
    public class LutronHWQSGatewayProtocol : AGatewayProtocol
    {
        #region Fields
        private bool _isLoginDone = false;
        private bool _monitoringCommandSent = false;
        private string _host;

        private readonly Dictionary<int, ALutronSwitchingDevice> _switches =
            new Dictionary<int, ALutronSwitchingDevice>();

        private readonly Dictionary<string, ALutronMotorDevice> _motors =
                   new Dictionary<string, ALutronMotorDevice>();
        private readonly Dictionary<int, CTimer> _timerList = new Dictionary<int, CTimer>();
        private CCriticalSection _pairedDevicesLock = new CCriticalSection();

        #endregion

        #region Initialization

        public LutronHWQSGatewayProtocol(ISerialTransport transport, byte id, string host)
            : base(transport, id)
        {
            ValidateResponse = GatewayValidateResponse;
            _host = host;
            SendDiscoveryRequest();
        }

        private ValidatedRxData GatewayValidateResponse(string response, CommonCommandGroupType commandGroup)
        {
            return new ValidatedRxData(true, response);
        }

        private void SendDiscoveryRequest()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                using (XmlTextReader reader = new XmlTextReader($"http://{_host}/DbXmlInfo.xml"))
                {
                    var project = (Project)serializer.Deserialize(reader);
                    var (shadeGroup, switches) = SerializeHelper.GetShadeGroupsAndSwitch(project.Areas);
                    foreach (var group in shadeGroup)
                    {
                        var device = new ALutronMotorDevice(group.Key, group.Key, group.Value);
                        device.SetConnectionStatus(true);
                        AddMotorPairedDevice(device);
                        device.MotorActionEvent += MotorActionEventHandler;
                    }

                    foreach (var device in switches)
                    {
                        device.SetConnectionStatus(true);
                        AddSwitchPairedDevice(device);
                        device.PowerStateChangedEvent += PowerStateChangeEventHandler;
                    }
                }
            }
            catch (Exception err)
            {
                // This would catch the network error
                Logger.Error(err.Message);
            }
        }
        #endregion

        #region Log Helper
        private void InfoLog(string title, string message)
        {
            if (EnableLogging) Log($"{title}: {message}");
        }

        private void ErrorLog(string error, string message)
        {
            if (EnableLogging) Log($"{error}: {message}");
        }
        #endregion

        #region Event Handler of Extension Devices
        private void MotorActionEventHandler(object sender, ValueEventArgs<MotorActionArgs> e)
        {
            InfoLog("MotorActionEventHandler", $"sent from extension driver, isLoginDone: {_isLoginDone}");
            // if (!_isLoginDone) return;
            if (e.Value.Shades.Count == 0) return;
            SendCommand(LutronCommand.MotorControlCommand(e.Value.Shades[0].Id, e.Value.Action));

            // Add delay to ensure all the command can be sent through transport class
            for (int i = 1; i < e.Value.Shades.Count; ++i)
            {
                int id = e.Value.Shades[i].Id;
                if (!_timerList.ContainsKey(id))
                {
                    CTimer timer = new CTimer(SendMotorControlCommand, new MotorTimerArgs(id, e.Value.Action), i * 250);
                    _timerList.Add(id, timer);
                }
            }
        }

        private void SendMotorControlCommand(object obj)
        {
            MotorTimerArgs args = obj as MotorTimerArgs;
            SendCommand(LutronCommand.MotorControlCommand(args.Id, args.Action));
            InfoLog("MotorActionEventHandler", $"{args.Id} set to action {args.Action}");
            if (_timerList.ContainsKey(args.Id))
            {
                _timerList.Remove(args.Id);
            }
        }

        private void PowerStateChangeEventHandler(object sender, ValueEventArgs<bool> e)
        {
            InfoLog("MotorActionEventHandler", $"sent from extension driver, isLoginDone: {_isLoginDone}");
            ALutronSwitchingDevice device = (ALutronSwitchingDevice)sender;
            int id = device.Id;
            // if (!_isLoginDone) return;
            SendCommand(LutronCommand.ZoneControlCommand(id, e.Value ? 100 : 0));
            InfoLog("PowerStateChangeEventHandler", $"Device sent to Gateway: Button pressed event handler: {e.Value}, Received from {id}");
        }
        #endregion

        #region Base Members

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            LutronRXEventArgs args = LutronRxParser.Parse(validatedData.Data);
            switch (args.Type)
            {
                case EventType.Login:
                    InfoLog("ChooseDeconstructMethod: Login Event", "Sending Login Command");
                    SendCommand(LutronCommand.LoginCommand);
                    _monitoringCommandSent = false;
                    break;
                case EventType.Password:
                    InfoLog("ChooseDeconstructMethod: Password Event", "Sending Password Command");
                    SendCommand(LutronCommand.PasswordCommand);
                    _monitoringCommandSent = false;
                    break;
                case EventType.Prompt:
                    InfoLog("ChooseDeconstructMethod: Prompt Received", "Set LoginDone to true");
                    _isLoginDone = true;
                    if (!_monitoringCommandSent)
                    {
                        InfoLog("ChooseDeconstructMethod", "Sending Monitoring Command");
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
                    InfoLog($"Feedback recevied from Gateway:", $"Brightness: {(int)brightness} (Integration ID: {device.Id}");
                    device.SetPower(brightness > 0);
                    break;
                case EventType.Error:
                    ErrorLog("ChooseDeconstructMethod: Command Error", args.Message);
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
        public override void Dispose()
        {
            try
            {
                _pairedDevicesLock.Enter();
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
                _pairedDevicesLock.Leave();
            }
            base.Dispose();
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

        private void RemovePairedSwitchingDevice(ALutronSwitchingDevice pairedDevice)
        {
            if (_switches.ContainsKey(pairedDevice.Id))
            {
                RemovePairedDevice(pairedDevice.Id.ToString());
            }
        }
        private void RemovePairedMotorDevice(ALutronMotorDevice pairedDevice)
        {
            if (_motors.ContainsKey(pairedDevice.Id))
            {
                RemovePairedDevice(pairedDevice.Id.ToString());
            }
        }
        #endregion
    }

    public class MotorTimerArgs
    {
        public int Id;
        public MotorAction Action;

        public MotorTimerArgs(int id, MotorAction action)
        {
            Id = id;
            Action = action;
        }
    }
}