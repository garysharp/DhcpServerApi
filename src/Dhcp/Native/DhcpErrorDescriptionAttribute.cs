using System;

namespace Dhcp.Native
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class DhcpErrorDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DhcpErrorDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
