using System;

namespace Dhcp
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OptionTagTypeAttribute : Attribute
    {
        public OptionTagTypes Type;

        public OptionTagTypeAttribute(OptionTagTypes Type)
        {
            this.Type = Type;
        }
    }
}
