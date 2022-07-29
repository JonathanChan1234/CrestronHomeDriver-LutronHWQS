using System;
using System.Text.RegularExpressions;

namespace LutronHWQSGateway
{
    public static class LutronRxParser
    {
        private const string LoginPattern = "login";
        private static Regex PasswordPattern = new Regex(@"password[: ]*$");
        private const string PromptString = "QNET>";
        private const string MonitoringResponse = "~MONITORING,5,1";
        private const string OutputPattern = "~OUTPUT";

        public static LutronRXEventArgs Parse(string rx)
        {
            if (rx.Contains(LoginPattern)) return new LutronRXEventArgs(EventType.Login);
            if (PasswordPattern.IsMatch(rx)) return new LutronRXEventArgs(EventType.Password);
            if (rx.Contains(PromptString)) return new LutronRXEventArgs(EventType.Prompt);
            if (rx.Contains(MonitoringResponse)) return new LutronRXEventArgs(EventType.Monitoring);
            if (rx.Contains(OutputPattern))
            {
                string[] parameters = rx.Split(new char[] { ',' });
                if (parameters.Length > 4) return new LutronRXEventArgs($"Invalid parameter length: {rx}");
                if (!int.TryParse(parameters[1], out int id)) return new LutronRXEventArgs($"Invalid Id {parameters[1]}");
                if (!int.TryParse(parameters[2], out int actionId)) return new LutronRXEventArgs($"Invalid ActionId {parameters[2]}");
                if (actionId != 1) return new LutronRXEventArgs($"Invalid action id {actionId}");
                if (!double.TryParse(parameters[3], out double brightness)) return new LutronRXEventArgs($"Invalid brightness {parameters[3]}");
                return new LutronRXEventArgs(id, brightness);
            }
            return new LutronRXEventArgs(EventType.Invalid);
        }
    }

    public class LutronRXEventArgs
    {
        public EventType Type;
        public int? Id;
        public double? Brightness;
        public string Message = "";

        public LutronRXEventArgs(EventType type)
        {
            Type = type;
        }

        public LutronRXEventArgs(int id, double brightness)
        {
            Type = EventType.Zone;
            Id = id;
            Brightness = brightness;
        }

        public LutronRXEventArgs(string message)
        {
            Type = EventType.Error;
            Message = message;
        }
    }

    public enum EventType
    {
        Login,
        Password,
        Prompt,
        Monitoring,
        Zone,
        Error,
        Invalid
    }
}
