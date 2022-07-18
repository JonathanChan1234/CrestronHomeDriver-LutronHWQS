using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Transports;
using System;

namespace LutronSwitchingDevice
{
    class ALutronSwitchingProtocol : ABaseDriverProtocol
    {
        public EventHandler<string> IconChangeEvent;
        public ALutronSwitchingProtocol(ISerialTransport transport, byte id) : base(transport, id)
        {
        }

        public override void SetUserAttribute(string attributeId, string attributeValue)
        {
            if (EnableLogging) Log($"attribute id: {attributeId} attributeValue: {attributeValue}");
            switch (attributeId)
            {
                case Constants.ICON_ATTRIBUTE:
                    IconChangeEvent?.Invoke(this, attributeValue);
                    break;
            }
        }

        protected override void ChooseDeconstructMethod(ValidatedRxData validatedData)
        {
            return;
        }

        protected override void ConnectionChangedEvent(bool connection)
        {
            return;
        }
    }
}
