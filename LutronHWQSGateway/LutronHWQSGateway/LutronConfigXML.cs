using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LutronHWQSGateway
{
    [XmlRoot(ElementName = "ProjectName")]
    public class ProjectName
    {

        [XmlAttribute(AttributeName = "ProjectName")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }
    }

    [XmlRoot(ElementName = "Dealer")]
    public class Dealer
    {

        [XmlAttribute(AttributeName = "AccountNumber")]
        public string AccountNumber { get; set; }

        [XmlAttribute(AttributeName = "UserID")]
        public string UserID { get; set; }
    }

    [XmlRoot(ElementName = "DealerInformation")]
    public class DealerInformation
    {

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Email")]
        public string Email { get; set; }

        [XmlAttribute(AttributeName = "Phone")]
        public string Phone { get; set; }
    }

    [XmlRoot(ElementName = "Connect")]
    public class Connect
    {

        [XmlElement(ElementName = "APIVer")]
        public string APIVer { get; set; }

        [XmlElement(ElementName = "PublicKey")]
        public string PublicKey { get; set; }

        [XmlElement(ElementName = "EncKey")]
        public object EncKey { get; set; }

        [XmlElement(ElementName = "Data")]
        public object Data { get; set; }
    }

    [XmlRoot(ElementName = "GlobalPreferences")]
    public class GlobalPreferences
    {

        [XmlElement(ElementName = "RaiseLowerDirection")]
        public string RaiseLowerDirection { get; set; }

        [XmlElement(ElementName = "LowerLevel")]
        public string LowerLevel { get; set; }

        [XmlElement(ElementName = "RaiseLevel")]
        public string RaiseLevel { get; set; }
    }

    [XmlRoot(ElementName = "Preset")]
    public class Preset
    {

        [XmlElement(ElementName = "PresetAssignments")]
        public PresetAssignments PresetAssignments { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Presets")]
    public class Presets
    {

        [XmlElement(ElementName = "Preset")]
        public List<Preset> Preset { get; set; }
    }

    [XmlRoot(ElementName = "Action")]
    public class Action
    {

        [XmlElement(ElementName = "Presets")]
        public Presets Presets { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "ActionType")]
        public string ActionType { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Actions")]
    public class Actions
    {

        [XmlElement(ElementName = "Action")]
        public List<Action> Action { get; set; }
    }

    [XmlRoot(ElementName = "Button")]
    public class Button
    {

        [XmlElement(ElementName = "Actions")]
        public Actions Actions { get; set; }

        [XmlElement(ElementName = "Notes")]
        public object Notes { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "Engraving")]
        public string Engraving { get; set; }

        [XmlAttribute(AttributeName = "ButtonType")]
        public string ButtonType { get; set; }

        [XmlAttribute(AttributeName = "PrimaryActionType")]
        public string PrimaryActionType { get; set; }

        [XmlAttribute(AttributeName = "SceneSaver")]
        public string SceneSaver { get; set; }

        [XmlAttribute(AttributeName = "CycleDim")]
        public string CycleDim { get; set; }

        [XmlAttribute(AttributeName = "AllowDoubleTap")]
        public string AllowDoubleTap { get; set; }

        [XmlAttribute(AttributeName = "StopIfMoving")]
        public string StopIfMoving { get; set; }

        [XmlAttribute(AttributeName = "ProgrammingModelID")]
        public string ProgrammingModelID { get; set; }

        [XmlAttribute(AttributeName = "LedLogic")]
        public string LedLogic { get; set; }

        [XmlAttribute(AttributeName = "ReverseLedLogic")]
        public string ReverseLedLogic { get; set; }

        [XmlAttribute(AttributeName = "Direction")]
        public string Direction { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Component")]
    public class Component
    {

        [XmlElement(ElementName = "Button")]
        public Button Button { get; set; }

        [XmlAttribute(AttributeName = "ComponentNumber")]
        public string ComponentNumber { get; set; }

        [XmlAttribute(AttributeName = "ComponentType")]
        public string ComponentType { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "LED")]
        public LED LED { get; set; }
    }

    [XmlRoot(ElementName = "Components")]
    public class Components
    {

        [XmlElement(ElementName = "Component")]
        public List<Component> Component { get; set; }
    }

    [XmlRoot(ElementName = "Device")]
    public class Device
    {

        [XmlElement(ElementName = "Components")]
        public Components Components { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "SerialNumber")]
        public string SerialNumber { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "DeviceType")]
        public string DeviceType { get; set; }

        [XmlAttribute(AttributeName = "GangPosition")]
        public string GangPosition { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Devices")]
    public class Devices
    {

        [XmlElement(ElementName = "Device")]
        public List<Device> Device { get; set; }
    }

    [XmlRoot(ElementName = "DeviceGroup")]
    public class DeviceGroup
    {

        [XmlElement(ElementName = "Devices")]
        public Devices Devices { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DeviceGroups")]
    public class DeviceGroups
    {

        [XmlElement(ElementName = "DeviceGroup")]
        public List<DeviceGroup> DeviceGroup { get; set; }
    }

    [XmlRoot(ElementName = "Scene")]
    public class Scene
    {

        [XmlElement(ElementName = "Presets")]
        public Presets Presets { get; set; }

        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Scenes")]
    public class Scenes
    {

        [XmlElement(ElementName = "Scene")]
        public List<Scene> Scene { get; set; }
    }

    [XmlRoot(ElementName = "Area")]
    public class Area
    {

        [XmlElement(ElementName = "Areas")]
        public Areas Areas { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "OccupancyGroupAssignedToID")]
        public string OccupancyGroupAssignedToID { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "DeviceGroups")]
        public object DeviceGroups { get; set; }

        [XmlElement(ElementName = "Scenes")]
        public object Scenes { get; set; }

        [XmlElement(ElementName = "ShadeGroups")]
        public object ShadeGroups { get; set; }

        [XmlElement(ElementName = "Outputs")]
        public Outputs Outputs { get; set; }
    }

    [XmlRoot(ElementName = "PresetAssignment")]
    public class PresetAssignment
    {

        [XmlElement(ElementName = "Delay")]
        public string Delay { get; set; }

        [XmlElement(ElementName = "Fade")]
        public string Fade { get; set; }

        [XmlElement(ElementName = "Level")]
        public string Level { get; set; }

        [XmlElement(ElementName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "AssignmentName")]
        public string AssignmentName { get; set; }

        [XmlAttribute(AttributeName = "AssignmentType")]
        public string AssignmentType { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "Number")]
        public string Number { get; set; }

        [XmlElement(ElementName = "TrimValue")]
        public string TrimValue { get; set; }
    }

    [XmlRoot(ElementName = "PresetAssignments")]
    public class PresetAssignments
    {

        [XmlElement(ElementName = "PresetAssignment")]
        public List<PresetAssignment> PresetAssignment { get; set; }
    }

    [XmlRoot(ElementName = "Output")]
    public class Output
    {

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "OutputType")]
        public string OutputType { get; set; }

        [XmlAttribute(AttributeName = "Wattage")]
        public string Wattage { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlAttribute(AttributeName = "AddressOnLink")]
        public string AddressOnLink { get; set; }

        [XmlAttribute(AttributeName = "Display")]
        public string Display { get; set; }
    }

    [XmlRoot(ElementName = "Outputs")]
    public class Outputs
    {

        [XmlElement(ElementName = "Output")]
        public List<Output> Output { get; set; }
    }

    [XmlRoot(ElementName = "LED")]
    public class LED
    {

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "isReverseLogic")]
        public string IsReverseLogic { get; set; }

        [XmlAttribute(AttributeName = "ProgrammingModelID")]
        public string ProgrammingModelID { get; set; }
    }

    [XmlRoot(ElementName = "Areas")]
    public class Areas
    {

        [XmlElement(ElementName = "Area")]
        public List<Area> Area { get; set; }
    }

    [XmlRoot(ElementName = "OccupancyGroup")]
    public class OccupancyGroup
    {

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "OccupancyGroupNumber")]
        public string OccupancyGroupNumber { get; set; }
    }

    [XmlRoot(ElementName = "OccupancyGroups")]
    public class OccupancyGroups
    {

        [XmlElement(ElementName = "OccupancyGroup")]
        public List<OccupancyGroup> OccupancyGroup { get; set; }
    }

    [XmlRoot(ElementName = "TimeclockMode")]
    public class TimeclockMode
    {

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "Index")]
        public string Index { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "TimeclockModes")]
    public class TimeclockModes
    {

        [XmlElement(ElementName = "TimeclockMode")]
        public List<TimeclockMode> TimeclockMode { get; set; }
    }

    [XmlRoot(ElementName = "Timeclock")]
    public class Timeclock
    {

        [XmlElement(ElementName = "TimeClockEvents")]
        public TimeClockEvents TimeClockEvents { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "TimeclockModes")]
        public TimeclockModes TimeclockModes { get; set; }
    }

    [XmlRoot(ElementName = "TimeClockEvent")]
    public class TimeClockEvent
    {

        [XmlElement(ElementName = "Actions")]
        public Actions Actions { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "Time")]
        public string Time { get; set; }

        [XmlAttribute(AttributeName = "EventNumber")]
        public string EventNumber { get; set; }

        [XmlAttribute(AttributeName = "ScheduleType")]
        public string ScheduleType { get; set; }

        [XmlAttribute(AttributeName = "Days")]
        public string Days { get; set; }

        [XmlAttribute(AttributeName = "Mode")]
        public string Mode { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "TimeClockEvents")]
    public class TimeClockEvents
    {

        [XmlElement(ElementName = "TimeClockEvent")]
        public List<TimeClockEvent> TimeClockEvent { get; set; }
    }

    [XmlRoot(ElementName = "Timeclocks")]
    public class Timeclocks
    {

        [XmlElement(ElementName = "Timeclock")]
        public List<Timeclock> Timeclock { get; set; }
    }

    [XmlRoot(ElementName = "HVACEvent")]
    public class HVACEvent
    {

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "EventNumber")]
        public string EventNumber { get; set; }

        [XmlAttribute(AttributeName = "Time")]
        public string Time { get; set; }

        [XmlAttribute(AttributeName = "HeatingSetPoint")]
        public string HeatingSetPoint { get; set; }

        [XmlAttribute(AttributeName = "CoolingSetPoint")]
        public string CoolingSetPoint { get; set; }

        [XmlAttribute(AttributeName = "HeatingSetPointFractional")]
        public string HeatingSetPointFractional { get; set; }

        [XmlAttribute(AttributeName = "CoolingSetPointFractional")]
        public string CoolingSetPointFractional { get; set; }

        [XmlAttribute(AttributeName = "EventEnabled")]
        public string EventEnabled { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }
    }

    [XmlRoot(ElementName = "HVACEvents")]
    public class HVACEvents
    {

        [XmlElement(ElementName = "HVACEvent")]
        public List<HVACEvent> HVACEvent { get; set; }
    }

    [XmlRoot(ElementName = "HVACSchedule")]
    public class HVACSchedule
    {

        [XmlElement(ElementName = "HVACEvents")]
        public HVACEvents HVACEvents { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "ScheduleNumber")]
        public string ScheduleNumber { get; set; }

        [XmlAttribute(AttributeName = "Days")]
        public string Days { get; set; }

        [XmlAttribute(AttributeName = "Schedule_Enabled")]
        public string ScheduleEnabled { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }
    }

    [XmlRoot(ElementName = "HVACSchedules")]
    public class HVACSchedules
    {

        [XmlElement(ElementName = "HVACSchedule")]
        public List<HVACSchedule> HVACSchedule { get; set; }
    }

    [XmlRoot(ElementName = "HVAC")]
    public class HVAC
    {

        [XmlElement(ElementName = "HVACSchedules")]
        public HVACSchedules HVACSchedules { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "AffectedAreaIDs")]
        public string AffectedAreaIDs { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlAttribute(AttributeName = "MinCoolSet")]
        public string MinCoolSet { get; set; }

        [XmlAttribute(AttributeName = "MaxCoolSet")]
        public string MaxCoolSet { get; set; }

        [XmlAttribute(AttributeName = "MinHeatSet")]
        public string MinHeatSet { get; set; }

        [XmlAttribute(AttributeName = "MaxHeatSet")]
        public string MaxHeatSet { get; set; }

        [XmlAttribute(AttributeName = "HeatCoolDelta")]
        public string HeatCoolDelta { get; set; }

        [XmlAttribute(AttributeName = "TemperatureUnits")]
        public string TemperatureUnits { get; set; }

        [XmlAttribute(AttributeName = "AvailableOperatingModes")]
        public string AvailableOperatingModes { get; set; }

        [XmlAttribute(AttributeName = "AvailableFanModes")]
        public string AvailableFanModes { get; set; }

        [XmlAttribute(AttributeName = "AvailableMiscFeatures")]
        public string AvailableMiscFeatures { get; set; }

        [XmlAttribute(AttributeName = "ControlType")]
        public string ControlType { get; set; }
    }

    [XmlRoot(ElementName = "HVACs")]
    public class HVACs
    {

        [XmlElement(ElementName = "HVAC")]
        public List<HVAC> HVAC { get; set; }
    }

    [XmlRoot(ElementName = "GreenModeStep")]
    public class GreenModeStep
    {

        [XmlElement(ElementName = "Presets")]
        public Presets Presets { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "UUID")]
        public string UUID { get; set; }

        [XmlAttribute(AttributeName = "StepNumber")]
        public string StepNumber { get; set; }

        [XmlAttribute(AttributeName = "SortOrder")]
        public string SortOrder { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "GreenModeSteps")]
    public class GreenModeSteps
    {

        [XmlElement(ElementName = "GreenModeStep")]
        public List<GreenModeStep> GreenModeStep { get; set; }
    }

    [XmlRoot(ElementName = "GreenMode")]
    public class GreenMode
    {

        [XmlElement(ElementName = "GreenModeSteps")]
        public GreenModeSteps GreenModeSteps { get; set; }

        [XmlAttribute(AttributeName = "IntegrationID")]
        public string IntegrationID { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "GreenModes")]
    public class GreenModes
    {

        [XmlElement(ElementName = "GreenMode")]
        public GreenMode GreenMode { get; set; }
    }

    [XmlRoot(ElementName = "Project")]
    public class Project
    {

        [XmlElement(ElementName = "ProjectName")]
        public ProjectName ProjectName { get; set; }

        [XmlElement(ElementName = "Dealer")]
        public Dealer Dealer { get; set; }

        [XmlElement(ElementName = "DealerInformation")]
        public DealerInformation DealerInformation { get; set; }

        [XmlElement(ElementName = "Latitude")]
        public string Latitude { get; set; }

        [XmlElement(ElementName = "Longitude")]
        public string Longitude { get; set; }

        [XmlElement(ElementName = "Copyright")]
        public string Copyright { get; set; }

        [XmlElement(ElementName = "GUID")]
        public string GUID { get; set; }

        [XmlElement(ElementName = "ProductType")]
        public string ProductType { get; set; }

        [XmlElement(ElementName = "AppVer")]
        public string AppVer { get; set; }

        [XmlElement(ElementName = "XMLVer")]
        public string XMLVer { get; set; }

        [XmlElement(ElementName = "DbExportDate")]
        public string DbExportDate { get; set; }

        [XmlElement(ElementName = "DbExportTime")]
        public string DbExportTime { get; set; }

        [XmlElement(ElementName = "IsConnectEnabled")]
        public string IsConnectEnabled { get; set; }

        [XmlElement(ElementName = "Connect")]
        public Connect Connect { get; set; }

        [XmlElement(ElementName = "SpectrumScenes")]
        public object SpectrumScenes { get; set; }

        [XmlElement(ElementName = "GlobalPreferences")]
        public GlobalPreferences GlobalPreferences { get; set; }

        [XmlElement(ElementName = "Areas")]
        public Areas Areas { get; set; }

        [XmlElement(ElementName = "OccupancyGroups")]
        public OccupancyGroups OccupancyGroups { get; set; }

        [XmlElement(ElementName = "Timeclocks")]
        public Timeclocks Timeclocks { get; set; }

        [XmlElement(ElementName = "HVACs")]
        public HVACs HVACs { get; set; }

        [XmlElement(ElementName = "GreenModes")]
        public GreenModes GreenModes { get; set; }
    }

}
