using System;

namespace Dhcp.Native
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class DhcpErrorDescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DhcpErrorDescriptionAttribute(string Description)
        {
            this.Description = Description;
        }
    }
}
