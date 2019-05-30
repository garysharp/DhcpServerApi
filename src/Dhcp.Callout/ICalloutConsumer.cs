using System;

namespace Dhcp.Callout
{
    /// <summary>
    /// Interface inherited by DHCP Server Callout API Consumers. The library uses this interface to locate consumers.
    /// </summary>
    public interface ICalloutConsumer : IDisposable
    {
        /// <summary>
        /// Called during DhcpServerCalloutEntry.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363300.aspx
        /// </remarks>
        /// <param name="ApiVersion">Version of the DHCP Server API</param>
        void Initialize(int ApiVersion);

        /// <summary>
        /// The Control function (DhcpControlHook) is called whenever the DHCP Server service is
        /// started, stopped, paused or continued. This function should not block.
        /// </summary>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa363276(v=vs.85).aspx
        /// </remarks>
        /// <param name="ControlCode">Specifies the control event that triggered the notification.</param>
        void Control(CalloutControlCodes ControlCode);
    }
}
