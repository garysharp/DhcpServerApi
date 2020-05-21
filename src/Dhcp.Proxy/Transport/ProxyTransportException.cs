using System;

namespace Dhcp.Proxy.Transport
{

    [Serializable]
    public class ProxyTransportException : Exception
    {
        public ProxyTransportException() { }
        public ProxyTransportException(string message) : base(message) { }
        public ProxyTransportException(string message, Exception inner) : base(message, inner) { }
        protected ProxyTransportException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
