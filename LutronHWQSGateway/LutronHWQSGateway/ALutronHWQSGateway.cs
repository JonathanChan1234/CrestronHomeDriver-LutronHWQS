using Crestron.RAD.Common.BasicDriver;
using Crestron.RAD.Common.Interfaces;
using Crestron.RAD.Common.Transports;
using Crestron.RAD.DeviceTypes.Gateway;
using Crestron.SimplSharp;
using System;

namespace LutronHWQSGateway
{
    public class ALutronHWQSGateway : AGateway, ITcp
    {
        public ALutronHWQSGateway()
        {
            try
            {
                AddCapabilities();
            }
            catch (TypeLoadException)
            {
                if (EnableLogging) Log("Type Load Exception");
            }
        }
        public void AddCapabilities()
        {
            var tcp2Capability = new Tcp2Capability(Initialize);
            Capabilities.RegisterInterface(typeof(ITcp2), tcp2Capability);
        }

        public void Initialize(IPAddress ipAddress, int port)
        {
            var transport = new TcpTransport
            {
                EnableAutoReconnect = EnableAutoReconnect,
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug
            };
            transport.Initialize(ipAddress, port);
            ConnectionTransport = transport;

            Protocol = new LutronHWQSGatewayProtocol(ConnectionTransport, Id, ipAddress.ToString())
            {
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger
            };
        }

        private void Initialize(string address, int port)
        {
            var tcpTransport = new TcpTransport
            {
                EnableAutoReconnect = EnableAutoReconnect,
                EnableLogging = InternalEnableLogging,
                CustomLogger = InternalCustomLogger,
                EnableRxDebug = InternalEnableRxDebug,
                EnableTxDebug = InternalEnableTxDebug
            };
            tcpTransport.Initialize(address, port);
            ConnectionTransport = tcpTransport;
            Protocol = new LutronHWQSGatewayProtocol(tcpTransport, Id, address)
            {
                EnableLogging = EnableLogging,
                CustomLogger = CustomLogger
            };
        }

        public override void Connect()
        {
            base.Connect();
            ((LutronHWQSGatewayProtocol)Protocol).Connect();
        }

        public override void Disconnect()
        {
            base.Disconnect();
            ((LutronHWQSGatewayProtocol)Protocol).Disconnect();
        }
    }
}