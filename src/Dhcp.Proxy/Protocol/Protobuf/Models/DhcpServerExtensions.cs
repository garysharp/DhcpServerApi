using Dhcp.Proxy.Models;
using System.Collections.Generic;

namespace Dhcp.Proxy.Protocol.Protobuf.Models
{
    public partial class ConnectRequest
    {
        public static implicit operator ConnectRequest(string hostNameOrAddress) => new ConnectRequest() { HostNameOrAddress = hostNameOrAddress };
    }

    public partial class ConnectResponse
    {
        public static implicit operator ConnectModel(ConnectResponse response)
        {
            return new ConnectModel()
            {
                Address = (DhcpServerIpAddress)response.Address,
                Name = response.Name,
                VersionMajor = response.VersionMajor,
                VersionMinor = response.VersionMinor,
            };
        }
        public static implicit operator ConnectResponse(ConnectModel model)
        {
            return new ConnectResponse()
            {
                Address = (uint)model.Address,
                Name = model.Name,
                VersionMajor = model.VersionMajor,
                VersionMinor = model.VersionMinor,
            };
        }
    }

    public partial class GetAuditLogResponse
    {
        public static implicit operator GetAuditLogModel(GetAuditLogResponse response)
        {
            return new GetAuditLogModel()
            {
                AuditLogDirectory = response.AuditLogDirectory,
                DiskCheckInterval = response.DiskCheckInterval,
                MaxLogFilesSize = response.MaxLogFilesSize,
                MinSpaceOnDisk = response.MinSpaceOnDisk,
            };
        }
        public static implicit operator GetAuditLogResponse(GetAuditLogModel model)
        {
            return new GetAuditLogResponse()
            {
                AuditLogDirectory = model.AuditLogDirectory,
                DiskCheckInterval = model.DiskCheckInterval,
                MaxLogFilesSize = model.MaxLogFilesSize,
                MinSpaceOnDisk = model.MinSpaceOnDisk,
            };
        }
    }

    public partial class GetProxyVersionResponse
    {
        public GetProxyVersionResponse(int version)
        {
            Version = version;
        }
    }

    public partial class GetRemoteServerNamesResponse
    {
        public GetRemoteServerNamesResponse(IEnumerable<string> serverNames)
        {
            ServerName.AddRange(serverNames);
        }
    }

}
