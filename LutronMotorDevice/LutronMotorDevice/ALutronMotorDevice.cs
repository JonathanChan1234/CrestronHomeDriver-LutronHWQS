using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Text;

namespace LutronMotorDevice
{
    public class ALutronMotorDevice : AExtensionDevice
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


        private const string MotorIconKey = "MotorIcon";
        private const string MotorStateLabelKey = "MotorStateLabel";
        private const string RaiseLowerCommand = "RaiseLower";
        private const string RaiseCommand = "Raise";
        private const string LowerCommand = "Lower";
        private const string StopCommand = "Stop";

        private const string MotorRaiseIcon = "icShadesOpen";
        private const string MotorStopIcon = "icShadesSemiOpen";
        private const string MotorLowerIcon = "icShadesClosed";

        private int _integrationId;

        #region property
        private PropertyValue<string> _motorStateLabelProperty;
        #endregion

        private bool _raise = false;

        private readonly GatewayPairedDeviceInformation _pairedDeviceInfo;
        public event EventHandler<ValueEventArgs<MotorAction>> MotorActionEvent;

        public ALutronMotorDevice(int id, string name)
        {
            _integrationId = id;
            _name = name;

            _motorStateLabelProperty = CreateProperty<string>(new PropertyDefinition(MotorStateLabelKey, null, DevicePropertyType.String));

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
            var uiFilePath = Path.Combine(uiFolderPath, "MotorLoad.xml");

            if (!File.Exists(uiFilePath))
            {
                Log(string.Format("ERROR: Ui Definition file not found. Path: {0}", uiFilePath));
                return null;
            }

            if (EnableLogging)
                Log(string.Format("UI Definition file found. Path: '{0}'", uiFilePath));

            return File.ReadToEnd(uiFilePath, Encoding.UTF8);
        }

        protected override IOperationResult DoCommand(string command, string[] parameters)
        {
            switch (command)
            {
                case RaiseLowerCommand:
                    _raise = !_raise;
                    _motorStateLabelProperty.Value = _raise ? "Raise" : "Lower";
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(_raise ? MotorAction.Raise : MotorAction.Lower));
                    break;
                case RaiseCommand:
                    _raise = true;
                    _motorStateLabelProperty.Value = "Raise";
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Raise));
                    break;
                case StopCommand:
                    _motorStateLabelProperty.Value = "Stop";
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Stop));
                    break;
                case LowerCommand:
                    _raise = false;
                    _motorStateLabelProperty.Value = "Lower";
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Lower));
                    break;
            }
            Commit();
            return new OperationResult(OperationResultCode.Success);
        }

        private void SetMotorState(MotorAction action)
        {
            switch (action)
            {
                case MotorAction.Lower:
                    _motorStateLabelProperty.Value = "Lower";
                    break;
                case MotorAction.Raise:
                    _motorStateLabelProperty.Value = "Raise";
                    break;
                case MotorAction.Stop:
                    _motorStateLabelProperty.Value = "Stop";
                    break;
            }
            Commit();
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
        {
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
        {
            return new OperationResult(OperationResultCode.Error, "this method is not implemented");
        }

        public GatewayPairedDeviceInformation PairedDeviceInformation
        {
            get { return _pairedDeviceInfo; }
        }

        public void SetConnectionStatus(bool connected)
        {
            Connected = connected;
        }

        [ProgrammableOperation("Raise")]
        public void Raise()
        {
            SetMotorState(MotorAction.Raise);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Raise));
        }

        [ProgrammableOperation("Stop")]
        public void Stop()
        {
            SetMotorState(MotorAction.Stop);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Stop));
        }

        [ProgrammableOperation("Lower")]
        public void Lower()
        {
            SetMotorState(MotorAction.Lower);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorAction>(MotorAction.Lower));
        }
    }

    public enum MotorAction
    {
        Raise,
        Stop,
        Lower
    }
}
