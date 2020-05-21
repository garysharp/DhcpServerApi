using Google.Protobuf;
using System;

namespace Dhcp.Proxy.Protocol.Protobuf.Models
{
    public static class ProtobufExtensions
    {
        public static T ReadMessage<T>(this CodedInputStream stream) where T : IMessage
        {
            var result = Activator.CreateInstance<T>();
            result.MergeFrom(stream);
            return result;
        }
    }
}
