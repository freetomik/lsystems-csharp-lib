using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LSystems
{
    internal static class ExtensionMethods
    {
        public static List<object> ObjectAsList(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj is IEnumerable)
            {
                return (from object o in (obj as IEnumerable) select o).ToList();
            }
            else
            {
                return new List<object>() { obj };
            }
        }

        public static IEnumerable EnumerateBackward(this Type[] types)
        {
            for (int i = types.Length - 1; i >= 0; --i)
            {            
                yield return types[i];
            }
        }

        public static bool FindEndOfBranch(this IEnumerator it, Type startType, Type endType)
        {
            int scopeDepth = 1;
            while (it.MoveNext())
            {
                if (it.Current.GetType() == startType)
                {
                    ++scopeDepth;
                }
                else if (it.Current.GetType() == endType)
                {
                    --scopeDepth;
                    if (scopeDepth == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
