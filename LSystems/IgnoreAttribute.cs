using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LSystems
{
    public class IgnoreAttribute : Attribute
    {
        public Type[] Types { get; private set; }

        public IgnoreAttribute(Type[] types)
        {
            this.Types = types;
        }

        internal static Type[] GetIgnoredTypes(MemberInfo memberInfo)
        {
            var attribute =
                (IgnoreAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(IgnoreAttribute), false);

            if (attribute != null)
            {
                return (Type[])attribute.Types.Clone();
            }
            else
            {
                return new Type[] { };
            }
        }
    }
}
