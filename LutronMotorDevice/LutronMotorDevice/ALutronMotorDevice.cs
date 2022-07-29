using Crestron.RAD.Common.Attributes.Programming;
using Crestron.RAD.Common.Enums;
using Crestron.RAD.Common.Events;
using Crestron.RAD.Common.Interfaces.ExtensionDevice;
using Crestron.RAD.DeviceTypes.ExtensionDevice;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronMotorDevice
{
    public class ALutronMotorDevice : AExtensionDevice
    {
        private readonly string _name;

        public new string Id
        {
            get { return _id; }
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

        private const string MotorStateLabelKey = "MotorStateLabel";

        private const string RaiseLowerCommand = "RaiseLower";
        private const string RaiseAllCommand = "RaiseAll";
        private const string LowerAllCommand = "LowerAll";
        private const string StopAllCommand = "StopAll";
        private const string RaiseCommand = "Raise";
        private const string LowerCommand = "Lower";
        private const string StopCommand = "Stop";

        private const string ShadeKey = "Shade";
        private const string ShadeNameKey = "Name";
        private const string ShadeIdKey = "Id";
        private const string ShadeListKey = "ShadeList";

        private string _id;
        private List<Shade> _shades;

        #region property
        private ALutronMotorProtocol _protocol;
        private PropertyValue<string> _motorStateLabelProperty;

        private ClassDefinition _shadeClass;
        private ObjectList _shadeList;
        #endregion

        private bool _raise = false;

        private readonly GatewayPairedDeviceInformation _pairedDeviceInfo;
        public event EventHandler<ValueEventArgs<MotorActionArgs>> MotorActionEvent;
        private IDictionary<int, ObjectValue> _shadeObjectInstance = new Dictionary<int, ObjectValue>();
        private IDictionary<string, Shade> _shadeDict = new Dictionary<string, Shade>();

        public ALutronMotorDevice(string id, string name, List<Shade> shades)
        {
            _id = id;
            _name = name;
            _shades = shades;

            _pairedDeviceInfo = new GatewayPairedDeviceInformation(
            id,
            name,
            Description,
            Manufacturer,
            BaseModel,
            DriverData.CrestronSerialDeviceApi.GeneralInformation.DeviceType,
            string.Empty);

            _motorStateLabelProperty = CreateProperty<string>(new PropertyDefinition(MotorStateLabelKey, string.Empty, DevicePropertyType.String));

            _shadeClass = CreateClassDefinition(ShadeKey);
            _shadeClass.AddProperty(new PropertyDefinition(ShadeNameKey, string.Empty, DevicePropertyType.String));
            _shadeClass.AddProperty(new PropertyDefinition(ShadeIdKey, string.Empty, DevicePropertyType.Int32));
            _shadeList = CreateList(new PropertyDefinition(ShadeListKey, string.Empty, DevicePropertyType.ObjectList, _shadeClass));

            InitShadeList(shades);
            ConnectionTransport = new DummyTransport();
            _protocol = new ALutronMotorProtocol(ConnectionTransport, 0x1);
            _protocol.MotorNameChangeEvent += MotorNameChangeEventHandler;
            DeviceProtocol = _protocol;
        }

        public void MotorNameChangeEventHandler(object sender, MotorNameChangeEventArgs args)
        {
            if (!int.TryParse(args.Id, out int id)) return;
            if (!_shadeObjectInstance.TryGetValue(id, out ObjectValue obj)) return;
            UpdateShade(obj, id, args.Name);
        }

        private void UpdateShade(ObjectValue obj, int id, string name)
        {
            if (EnableLogging) Log($"Update shade (id: {id}) name to {name}");
            obj.GetValue<int>(ShadeIdKey).Value = id;
            obj.GetValue<string>(ShadeNameKey).Value = name;
            Commit();
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

        private void InitShadeList(List<Shade> shades)
        {
            _shadeList.Clear();
            foreach (var shade in shades)
            {
                var obj = CreateObject(_shadeClass);
                obj.GetValue<string>(ShadeNameKey).Value = shade.Name;
                obj.GetValue<int>(ShadeIdKey).Value = shade.Id;
                _shadeList.AddObject(obj);

                _shadeObjectInstance.Add(shade.Id, obj);
                _shadeDict.Add(obj.Id, shade);

                AddUserAttribute(
               UserAttributeType.Custom,
               shade.Id.ToString(),
               $"Name for {shade.Name}",
               "",
               true,
               UserAttributeRequiredForConnectionType.After,
               UserAttributeDataType.String,
               shade.Name);
            }
            Commit();
        }

        private void ControlShade(string[] parameters, MotorAction action)
        {
            if (parameters.Length < 1) return;
            var idString = parameters[0];
            if (!int.TryParse(idString, out int id)) return;
            List<Shade> shades = new List<Shade>
            {
                new Shade(id, "shade")
            };
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(shades, action)));
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
                    var action = _raise ? MotorAction.Raise : MotorAction.Lower;
                    SetMotorState(action);
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, action)));
                    break;
                case RaiseAllCommand:
                    _raise = true;
                    SetMotorState(MotorAction.Raise);
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Raise)));
                    break;
                case StopAllCommand:
                    SetMotorState(MotorAction.Stop);
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Stop)));
                    break;
                case LowerAllCommand:
                    _raise = false;
                    SetMotorState(MotorAction.Lower);
                    MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Lower)));
                    break;
                case RaiseCommand:
                    ControlShade(parameters, MotorAction.Raise);
                    break;
                case StopCommand:
                    ControlShade(parameters, MotorAction.Stop);
                    break;
                case LowerCommand:
                    ControlShade(parameters, MotorAction.Lower);
                    break;
            }
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string propertyKey, T value)
        {
            return new OperationResult(OperationResultCode.Success);
        }

        protected override IOperationResult SetDriverPropertyValue<T>(string objectId, string propertyKey, T value)
        {
            if (!_shadeDict.TryGetValue(objectId, out Shade shade)) return new OperationResult(OperationResultCode.Error, "The shade you are trying to modify does not exist.");
            switch (propertyKey)
            {
                case ShadeNameKey:
                    if (!_shadeObjectInstance.TryGetValue(shade.Id, out ObjectValue obj)) break;
                    var name = value as string;
                    SetUserAttribute(shade.Id.ToString(), name);
                    UpdateShade(obj, shade.Id, name);
                    return new OperationResult(OperationResultCode.Success);
            }
            return new OperationResult(OperationResultCode.Error, "The shade you are trying to modify does not exist.");
        }


        public void SetConnectionStatus(bool connected)
        {
            Connected = connected;
        }

        [ProgrammableOperation("Raise All")]
        public void RaiseAll()
        {
            SetMotorState(MotorAction.Raise);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Raise)));
        }

        [ProgrammableOperation("Stop All")]
        public void StopAll()
        {
            SetMotorState(MotorAction.Stop);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Stop)));
        }

        [ProgrammableOperation("Lower All")]
        public void LowerAll()
        {
            SetMotorState(MotorAction.Lower);
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(_shades, MotorAction.Lower)));
        }

        public List<string> GetShadeNames()
        {
            List<string> shadeNameList = new List<string>();
            foreach (var shade in _shadeObjectInstance)
            {
                shadeNameList.Add(shade.Value.GetValue<string>(ShadeNameKey).Value);
            }
            return shadeNameList;
        }

        private Shade GetShadeByName(string name)
        {
            foreach (var shade in _shadeObjectInstance)
            {
                if (!shade.Value.GetValue<string>(ShadeNameKey).Value.Equals(name)) continue;
                if (_shadeDict.TryGetValue(shade.Value.Id, out Shade obj)) return obj;
            }
            return null;
        }

        [ProgrammableOperation("Raise")]
        public void Raise(
            [Display("Shade Name")]
                    [DynamicAvailableValues(nameof(ALutronMotorDevice), nameof(GetShadeNames), OperationType.Method)]
                    string name
            )
        {
            var shade = GetShadeByName(name);
            if (shade == null) return;
            List<Shade> list = new List<Shade> { shade };
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(list, MotorAction.Raise)));
        }

        [ProgrammableOperation("Lower")]
        public void Lower(
            [Display("Shade Name")]
                    [DynamicAvailableValues(nameof(ALutronMotorDevice), nameof(GetShadeNames), OperationType.Method)]
                    string name
            )
        {
            var shade = GetShadeByName(name);
            if (shade == null) return;
            List<Shade> list = new List<Shade> { shade };
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(list, MotorAction.Lower)));
        }

        [ProgrammableOperation("Stop")]
        public void Stop(
            [Display("Shade Name")]
                    [DynamicAvailableValues(nameof(ALutronMotorDevice), nameof(GetShadeNames), OperationType.Method)]
                    string name
            )
        {
            var shade = GetShadeByName(name);
            if (shade == null) return;
            List<Shade> list = new List<Shade> { shade };
            MotorActionEvent?.Invoke(this, new ValueEventArgs<MotorActionArgs>(new MotorActionArgs(list, MotorAction.Stop)));
        }

        public GatewayPairedDeviceInformation PairedDeviceInformation
        {
            get { return _pairedDeviceInfo; }
        }
    }

    public class Shade
    {
        public int Id;
        public string Name;
        public Shade(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class MotorActionArgs
    {
        public List<Shade> Shades;
        public MotorAction Action;
        public MotorActionArgs(List<Shade> shades, MotorAction action)
        {
            Shades = shades;
            Action = action;
        }
    }

    public enum MotorAction
    {
        Raise,
        Stop,
        Lower
    }
}
