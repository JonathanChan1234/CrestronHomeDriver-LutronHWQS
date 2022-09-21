using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Enums;
using LutronMotorDevice;

namespace LutronHWQSGateway
{
    public static class LutronCommand
    {
        public static CommandSet LoginCommand = CommandSetBuilder("login", "lutron");
        public static CommandSet PasswordCommand = CommandSetBuilder("password", "integration");
        public static CommandSet MonitoringCommand = CommandSetBuilder("monitoring", "#MONITORING,5,1");
        public static CommandSet ZoneControlCommand(int id, int brightness)
        {
            return CommandSetBuilder("output", $"#OUTPUT,{id},1,{brightness},2");
        }

        public static CommandSet MotorControlCommand(int id, MotorAction action)
        {
            switch (action)
            {
                case MotorAction.Raise:
                    return CommandSetBuilder("motor", $"#OUTPUT,{id},2");
                case MotorAction.Stop:
                    return CommandSetBuilder("motor", $"#OUTPUT,{id},4");
                case MotorAction.Lower:
                    return CommandSetBuilder("motor", $"#OUTPUT,{id},3");
                default:
                    return CommandSetBuilder("motor", $"#OUTPUT,{id},4");
            }
        }

        private static CommandSet CommandSetBuilder(string name, string command)
        {
            return new CommandSet(
               name,
               $"{command}\r\n",
               CommonCommandGroupType.Other,
               null,
               false,
               CommandPriority.Normal,
               StandardCommandsEnum.NotAStandardCommand);
        }
    }
}
