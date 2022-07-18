using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Transports;
using System;

namespace LutronMotorDevice
{
    class ALutronMotorProtocol : ABaseDriverProtocol
    {
        public EventHandler<MotorNameChangeEventArgs> MotorNameChangeEvent;
        public ALutronMotorProtocol(ISerialTransport transport, byte id) : base(transport, id)
        {
        }

        public override void SetUserAttribute(string attributeId, string attributeValue)
        {
            if (EnableLogging) Log($"attribute id: {attributeId} attributeValue: {attributeValue}");
            MotorNameChangeEvent?.Invoke(this, new MotorNameChangeEventArgs(attributeId, attributeValue));
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
        }

        protected override void ConnectionChangedEvent(bool connection)
        {
        }
    }

    public class MotorNameChangeEventArgs
    {
        public string Id;
        public string Name;
        public MotorNameChangeEventArgs(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
