using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LSystems
{
    internal class DecompositionRule
    {
        public MethodInfo MethodInfo { get; private set; }
        public Type Type { get; private set; }        

        public static DecompositionRule GenerateRule(MethodInfo method)
        {
            DecompositionAttribute decomposition =
              (DecompositionAttribute)Attribute.GetCustomAttribute(method, typeof(DecompositionAttribute), false);

            if (decomposition == null)
            {
                return null;
            }

            if (method.ReturnType == typeof(void))
            {
                throw new InvalidOperationException("Decomposition rule must return object.");
            }            
            
            if (method.GetParameters().Count() != 1)
            {
                throw new InvalidOperationException("Wrong parameters number in decomposition function. Decomposition is performed on a single module.");
            }

            return new DecompositionRule()
            {
                MethodInfo = method,
                Type = method.GetParameters()[0].ParameterType
            };
        }
    }
}
