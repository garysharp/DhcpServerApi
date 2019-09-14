using System;

namespace Dhcp.Native
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class DhcpServerNativeErrorDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DhcpServerNativeErrorDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
