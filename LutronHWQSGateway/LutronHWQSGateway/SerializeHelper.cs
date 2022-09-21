using LutronMotorDevice;
using LutronSwitchingDevice;
using System.Collections.Generic;

namespace LutronHWQSGateway
{
    static class SerializeHelper
    {

        public static (IDictionary<string, List<Shade>> shadeGroup, List<ALutronSwitchingDevice> switches) GetShadeGroupsAndSwitch(Areas areas)
        {
            var shadeGroup = new Dictionary<string, List<Shade>>();
            var switches = new List<ALutronSwitchingDevice>();
            GetShadeGroupAndSwitchHelper(areas, "", shadeGroup, switches);
            return (shadeGroup, switches);
        }

        public static void GetShadeGroupAndSwitchHelper(Areas areas, string name, IDictionary<string, List<Shade>> shadeGroup, List<ALutronSwitchingDevice> switches)
        {
            // base case: if there the areas is null and there is no child under the current area
            if (areas == null) return;
            if (areas.Area == null) return;
            if (areas.Area.Count == 0) return;

            foreach (var area in areas.Area)
            {
                var areaName = $"{name}/{area.Name}";
                List<Shade> shades = new List<Shade>();
                foreach (var output in area.Outputs.Output)
                {
                    if (!int.TryParse(output.IntegrationID, out int id)) continue;
                    if (output.OutputType == "MOTOR")
                        shades.Add(new Shade(id, output.Name));
                    if (output.OutputType == "NON_DIM_INC")
                        switches.Add(new ALutronSwitchingDevice(id, output.Name, SwitchLoadType.Light));
                }
                if (shades.Count > 0)
                    shadeGroup.Add(areaName, shades);
                GetShadeGroupAndSwitchHelper(area.Areas, areaName, shadeGroup, switches);
            }
        }
    }
}
