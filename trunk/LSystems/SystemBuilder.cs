using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace LSystems
{
    public class SystemBuilder
    {
        public static System BuildSystem(object systemDefinition)
        {
            var productionRules = new List<ProductionRule>();
            var decompositionRules = new List<DecompositionRule>();
            var systemDefinitionType = systemDefinition.GetType();

            foreach (var method in systemDefinitionType.GetMethods())
            {
                var productionRule = ProductionRule.GenerateRule(method);
                if (productionRule != null)
                {
                    productionRules.Add(productionRule);
                    continue;
                }

                var decompositionRule = DecompositionRule.GenerateRule(method);
                if (decompositionRule != null)
                {
                    decompositionRules.Add(decompositionRule);
                    continue;
                }
            }

            return new System(productionRules, decompositionRules, systemDefinition);
        }        
    }
}