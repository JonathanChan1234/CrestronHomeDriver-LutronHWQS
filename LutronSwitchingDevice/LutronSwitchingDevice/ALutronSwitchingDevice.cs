using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Text;

namespace LutronSwitchingDevice
{
    public class ALutronSwitchingDevice : AExtensionDevice
    {
        private readonly string _name;

        public new int Id
        {
            get { return _integrationId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public new string Description
        {
            get { return "Lutron Paired Device"; }
        }


        public new string Manufacturer
        {
            get { return "Jonathan Chan"; }
        }

        public string Model
        {
            get { return "Sample Model"; }
        }

        public string DeviceType
        {
            get { return "Paired Device"; }
        }

        public string DeviceSubtype
        {
            get { return "Subtype"; }
        }


        private const string LoadIconKey = "LoadIcon";
        private const string LoadStateLabelKey = "LoadStateLabel";
        private const string ToggleLoadCommand = "ToggleLoad";
        private const string LoadStateKey = "LoadState";

        private const string ExhaustFanOnIcon = "icFanOn";
        private const string ExhaustFanOffIcon = "icFanOff";
        private const string HoodOnIcon = "icHoodOn";
        private const string HoodOffIcon = "icHoodOff";
        private const string WaterHeaterOnIcon = "icFireOn";
        private const string WaterHeaterOffIcon = "icFireOff";
        private const string LightOnIcon = "icLightsOnRegular";
        private const string LightOffIcon = "icLightsOffRegular";

        private int _integrationId;
        private bool _loadState;
        private SwitchLoadType _loadType;

        #region property
        private PropertyValue<string> _iconProperty;
        private PropertyValue<string> _loadStateLabelProperty;
        private PropertyValue<bool> _loadStateProperty;
        #endregion

        private readonly GatewayPairedDeviceInformation _pairedDeviceInfo;
        public event EventHandler<ValueEventArgs<bool>> PowerStateChangedEvent;

        public ALutronSwitchingDevice(int id, string name, SwitchLoadType loadType)
        {
            _integrationId = id;
            _name = name;
            _loadState = false;
            _loadType = loadType;

            _iconProperty = CreateProperty<string>(new PropertyDefinition(LoadIconKey, null, DevicePropertyType.String));
            _loadStateLabelProperty = CreateProperty<string>(new PropertyDefinition(LoadStateLabelKey, null, DevicePropertyType.String));
            _loadStateProperty = CreateProperty<bool>(new PropertyDefinition(LoadStateKey, null, DevicePropertyType.Boolean));

            _iconProperty.Value = GetLoadIcon(_loadState, _loadType);
            Commit();

            _pairedDeviceInfo = new GatewayPairedDeviceInformation(
            id.ToString(),
            name,
            Description,
            Manufacturer,
            BaseModel,
            DriverData.CrestronSerialDeviceApi.GeneralInformation.DeviceType,
            string.Empty);
        }

        protected override string GetUiDefinition(string uiFolderPath)
        {
            var uiFilePath = Path.Combine(uiFolderPath, "SwitchingLoad.xml");

            if (!File.Exists(uiFilePath))
            {
                Log(string.Format("ERROR: Ui Definition file not found. Path: {0}", uiFilePath));
                return null;
            }

            if (EnableLogging)
                Log(string.Format("UI Definition file found. Path: '{0}'", uiFilePath));

            return File.ReadToEnd(uiFilePath, Encoding.UTF8);
        }

        // Feedback received from the gateway driver
        public void SetPower(bool state)
        {
            _loadState = state;
            _loadStateProperty.Value = state;
            _loadStateLabelProperty.Value = _loadState ? "ON" : "OFF";
            _iconProperty.Value = GetLoadIcon(state, _loadType);
            if (EnableLogging) Log($"Set Switch Load ({_integrationId}) To: {state}");
            Commit();
        }

        private string GetLoadIcon(bool state, SwitchLoadType type)
        {
            switch (type)
            {
                case SwitchLoadType.ExhaustFan:
                    return state ? ExhaustFanOnIcon : ExhaustFanOffIcon;
                case SwitchLoadType.WaterHeater:
                    return state ? WaterHeaterOnIcon : WaterHeaterOffIcon;
                case SwitchLoadType.Hood:
                    return state ? HoodOnIcon : HoodOffIcon;
                case SwitchLoadType.Light:
                    return state ? LightOnIcon : LightOffIcon;
                default:
                    return state ? LightOnIcon : LightOffIcon;
            }
        }

        protected override IOperationResult DoCommand(string command, string[] parameters)
        {
            switch (command)
            {
                case ToggleLoadCommand:
                    bool state = !_loadState;
                    SetPower(state);
                    // Send the control value to the Gateway Driver
                    PowerStateChangedEvent?.Invoke(this, new ValueEventArgs<bool>(state));
                    break;
            }

            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
        {
            switch (propertyKey)
            {
                case LoadStateKey:
                    var state = value as bool?;
                    if (state == null) return new OperationResult(OperationResultCode.Error, "The value provided cannot be converted to bool");
                    SetPower((bool)state);
                    PowerStateChangedEvent?.Invoke(this, new ValueEventArgs<bool>((bool)state));
                    return new OperationResult(OperationResultCode.Success);
            }
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
        {
            return new OperationResult(OperationResultCode.Success);

        }

        public GatewayPairedDeviceInformation PairedDeviceInformation
        {
            get { return _pairedDeviceInfo; }
        }

        public void SetConnectionStatus(bool connected)
        {
            Connected = connected;
        }

        [ProgrammableOperation("On")]
        public void TurnOn()
        {
            SetPower(true);
            PowerStateChangedEvent?.Invoke(this, new ValueEventArgs<bool>(true));

        }

        [ProgrammableOperation("Off")]
        public void TurnOff()
        {
            SetPower(false);
            PowerStateChangedEvent?.Invoke(this, new ValueEventArgs<bool>(false));
        }
    }

    public enum SwitchLoadType
    {
        ExhaustFan,
        WaterHeater,
        Hood,
        Light
    }
}
