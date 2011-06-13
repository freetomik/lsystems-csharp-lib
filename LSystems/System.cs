using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace LSystems
{
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

                        // DEBUG!!!!!!!!!!!!!!!!!!!!!!
                        // TODO: may be create an event in the system,
                        // so all replace actions can be memorized or displayed
                        // with animation???
                        //for (int j = 0; j < currentString.Count; ++j)
                        //{
                        //    Debug.Write(" ");
                        //    if (j == i - (numReplacedModules - 1)) Debug.Write("<<<");
                        //    Debug.Write(currentString[j].GetType().Name.Split('.').Last());
                        //    if (j == i) Debug.Write(">>>");
                        //}
                        //Debug.Write(", " + rule.Method.Name + ", right: ");                        
                        //for (int j = 0; j < newString.Count; ++j)
                        //{
                        //    Debug.Write(" " + newString[j].GetType().Name.Split('.').Last());
                        //}
                        //Debug.WriteLine(string.Empty);
                        // DEBUG!!!!!!!!!!!!!!!!!!!!!!

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

                // DEBUG!!!!!!!!!!!!!!!!!!!!!!
                // TODO: may be create an event in the system,
                // so all replace actions can be memorized or displayed
                // with animation???
                //for (int j = 0; j < currentString.Count; ++j)
                //{
                //    Debug.Write(" ");
                //    if (j == i) Debug.Write("<<<");
                //    Debug.Write(currentString[j].GetType().Name.Split('.').Last());
                //    if (j == i) Debug.Write(">>>");
                //}
                //Debug.Write(", copy, right: ");
                //for (int j = 0; j < newString.Count; ++j)
                //{
                //    Debug.Write(" " + newString[j].GetType().Name.Split('.').Last());                    
                //}
                //Debug.WriteLine(string.Empty);
                // DEBUG!!!!!!!!!!!!!!!!!!!!!!

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

            if (this.decRules.Count == 0)
            {
                return;
            }

            List<object> newString = new List<object>();
            foreach (var m in this.currentString)
            {
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
                            if (isFinished)
                            {
                                foreach (var newModule in newModules)
                                {
                                    if (this.decRules.ContainsKey(newModule.GetType()))
                                    {
                                        isFinished = false;
                                        break;
                                    }
                                }
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
    }
}
