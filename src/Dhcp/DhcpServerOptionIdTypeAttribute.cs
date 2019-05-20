using System;

namespace Dhcp
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DhcpServerOptionIdTypeAttribute : Attribute
    {
        public DhcpServerOptionIdTypes Type;

        public DhcpServerOptionIdTypeAttribute(DhcpServerOptionIdTypes type)
        {
            Type = type;
        }
    }
}
