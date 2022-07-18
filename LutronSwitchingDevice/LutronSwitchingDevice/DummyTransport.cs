
using Crestron.RAD.Common.Transports;

namespace LutronSwitchingDevice
{
    class DummyTransport : ATransportDriver
    {
        public DummyTransport()
        {
            IsConnected = true;
        }

        public override void SendMethod(string message, object[] paramaters)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}
