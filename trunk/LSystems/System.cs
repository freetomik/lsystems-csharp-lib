using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace LSystems
{
    // TODO: may be create an ReplacementDone event in the system,
    // so all replace actions can be memorized or displayed
    // with animation???
                        
    public class System
    {
        private List<ProductionRule> prodRules;
        private Dictionary<Type, List<DecompositionRule>> decRules;
        private List<object> currentString;
        private object systemDefinition;

        internal System(IEnumerable<ProductionRule> productionRules, IEnumerable<DecompositionRule> decompositionRules, object systemDefinition)
        {
            this.currentString = new List<object>();
            this.systemDefinition = systemDefinition;
            this.prodRules = productionRules.ToList();
            this.decRules = new Dictionary<Type,List<DecompositionRule>>();
            foreach (var d in decompositionRules)
            {
                if (!this.decRules.ContainsKey(d.Type))
                {
                    this.decRules[d.Type] = new List<DecompositionRule>();
                }
                this.decRules[d.Type].Add(d);
            }            
        }

        public object Definition
        {
            get { return systemDefinition; }
        }

        public object String
        {
            get
            {
                return this.currentString;
            }

            set
            {
                this.currentString = value.ObjectAsList();
            }
        }

        public void RewriteLeftToRight()
        {
            List<object> newString = new List<object>();

            for (int i = 0; i < this.currentString.Count; ++i)
            {
                foreach (var rule in this.prodRules)
                {
                    object replacement = null;
                    int numReplacedModules = 1;

                    if (rule.TryLeftToRight(currentString, i, newString, systemDefinition, ref replacement, ref numReplacedModules))
                    {
                        i += numReplacedModules - 1;
                        
                        if (replacement is EmptyModule)
                        {
                            // Do nothing, Strict is removed from new string.
                            goto endOfReplacement;
                        }
                        else
                        {
                            // Have some replacement.
                            newString.InsertRange(newString.Count, replacement.ObjectAsList());                            
                            goto endOfReplacement;
                        }   
                    }                                     
                }

                // No replacement were found, 
                // copy old module to the new string.
                newString.Add(this.currentString[i]);

            endOfReplacement: ;
            }

            this.currentString = newString;
        }

        public void RewriteRightToLeft()
        {
            List<object> newString = new List<object>();

            for (int i = this.currentString.Count - 1; i >= 0; --i)
            {
                foreach (var rule in this.prodRules)
                {
                    object replacement = null;
                    int numReplacedModules = 1;

                    if (rule.TryRightToLeft(currentString, i, newString, systemDefinition, ref replacement, ref numReplacedModules))
                    {
                        i -= numReplacedModules - 1;
                        
                        if (replacement is EmptyModule)
                        {
                            // Do nothing, Strict is removed from new string.
                            goto endOfReplacement;
                        }
                        else
                        {
                            // Have some replacement.
                            newString.InsertRange(0, replacement.ObjectAsList());
                            goto endOfReplacement;
                        }
                    }
                }               
    
                // No replacement were found, 
                // copy old module to the new string.
                newString.Insert(0, this.currentString[i]);

            endOfReplacement: ;
            }            

            this.currentString = newString;
        }        

        public void Decomposite()
        {
            bool isFinished;

            this.Decomposite(out isFinished);
        }

        public void Decomposite(out bool isFinished)
        {
            isFinished = true;           

            List<object> newString = new List<object>();
            
            ForwardEnumerator it = new ForwardEnumerator(this.currentString, 0);
            while (it.MoveNext())
            {
                object m = it.Current;
                if (m is CutModule)
                {
                    // Apply cutting rule - goto end of current branch.
                    it.FindEndOfBranch(typeof(StartBranchModule), typeof(EndBranchModule));

                    // Current module should be end of branch.
                    m = it.Current;
                }

                if (this.decRules.ContainsKey(m.GetType()))
                {
                    foreach (var d in this.decRules[m.GetType()])
                    {
                        object result = d.MethodInfo.Invoke(this.systemDefinition, new object[] { m });
                        if (result is EmptyModule)
                        {
                            // Do nothing, Strict is removed from new string.
                            goto endOfDecomposition;
                        }
                        else if (result != null)
                        {
                            // Have some replacement.
                            List<object> newModules = result.ObjectAsList();
                            newString.InsertRange(newString.Count, newModules);

                            // Check if we are still not finished.
                            if (isFinished && CanBeDecomposed(newModules))
                            {
                                isFinished = false;
                            }

                            goto endOfDecomposition;
                        }
                    }
                }

                // No decomposition were done.
                newString.Add(m);

            endOfDecomposition: ;
            }

            this.String = newString;
        }

        private bool CanBeDecomposed(List<object> newModules)
        {
            foreach (var newModule in newModules)
            {
                if (this.decRules.ContainsKey(newModule.GetType()))
                {
                    return true;
                }
            }
            return false;
        }        
    }
}
