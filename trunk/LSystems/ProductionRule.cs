using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace LSystems
{
    public class ProductionRule
    {
        protected Type[] NewLeft { get; set; }
        protected Type[] Left { get; set; }
        protected Type[] Strict { get; set; }
        protected Type[] Right { get; set; }
        protected Type[] NewRight { get; set; }
        public MethodInfo Method { get; set; }
        protected Type[] IgnoreList { get; set; }

        public static ProductionRule GenerateRule(MethodInfo method)
        {
            ProductionAttribute production = 
                (ProductionAttribute)Attribute.GetCustomAttribute(method, typeof(ProductionAttribute), false);

            if (production == null)
            {
                return null;
            }

            Type[] newLeft, left, strict, right, newRight;
            production.GetTypes(method, out newLeft, out left, out strict, out right, out newRight);
                            
            var ignoreList = IgnoreAttribute.GetIgnoredTypes(method);            
            foreach(var p in method.GetParameters())
            {
                if (ignoreList.Contains(p.ParameterType))
                {
                    throw new InvalidOperationException("Ignored type is production function parameters list.");
                }            
            }

            Array.Reverse(newLeft);
            Array.Reverse(left);

            return new ProductionRule() 
            {
                Method = method,
                NewLeft = newLeft,
                Left = left,
                Strict = strict,
                Right = right,
                NewRight = newRight,
                IgnoreList = ignoreList
            };
        }                   

        public bool TryLeftToRight(
            List<object> currentString, 
            int currentIndex, 
            List<object> newLeftString,             
            object systemDefinition,
            ref object result,
            ref int numReplacedModules)
        {
            List<object> parameters = new List<object>();

            // Check left predecessors.
            if (!this.IsLeftMatch(new BackwardEnumerator(newLeftString, newLeftString.Count - 1), this.NewLeft.GetEnumerator(), parameters))
            {
                return false;                
            }

            if (!this.IsLeftMatch(new BackwardEnumerator(currentString, currentIndex - 1), this.Left.GetEnumerator(), parameters))
            {
                return false;
            }

            // Reverse parameters list, because it is in reverse order.
            parameters.Reverse();

            // Check strict predecessor.
            var it = new ForwardEnumerator(currentString, currentIndex);
            if (!this.IsSimpleMatch(it, this.Strict.GetEnumerator(), parameters))
            {
                return false;
            }

            // Memorize length of strict part.
            numReplacedModules = it.NumMoves;

            // Check right predecessor.
            if (!this.IsRightMatch(it, this.Right.GetEnumerator(), parameters))
            {
                return false;
            }

            // Invoke rule.
            result = this.Method.Invoke(systemDefinition, parameters.ToArray());                        

            return result != null;
        }

        public bool TryRightToLeft(
            List<object> currentString,
            int currentIndex,
            List<object> newRightString,
            object systemDefinition,
            ref object result,
            ref int numReplacedModules)
        {
            List<object> rightParameters = new List<object>();
            List<object> leftParameters = new List<object>();

            // Check newRight predecessor.
            if (!this.IsRightMatch(new ForwardEnumerator(newRightString, 0), this.NewRight.GetEnumerator(), rightParameters))
            {
                return false;
            }

            // Check right predecessor.
            if (!this.IsRightMatch(new ForwardEnumerator(currentString, currentIndex + 1), this.Right.GetEnumerator(), rightParameters))
            {
                return false;
            }

            // Check strict predecessor.
            var it = new BackwardEnumerator(currentString, currentIndex);
            if (!this.IsSimpleMatch(it, this.Strict.EnumerateBackward().GetEnumerator(), leftParameters))
            {
                return false;
            }

            // Memorize length of strict part.
            numReplacedModules = it.NumMoves;

            // Check right predecessor.
            if (!this.IsLeftMatch(it, this.Left.GetEnumerator(), leftParameters))
            {
                return false;
            }

            // Build final list of parameters.
            leftParameters.Reverse();

            var parameters = leftParameters.Concat(rightParameters);

            // Invoke rule.
            result = this.Method.Invoke(systemDefinition, parameters.ToArray());

            return result != null;
        }        

        private bool IsSimpleMatch(IEnumerator modules, IEnumerator types, List<object> parameters)
        {
            while (types.MoveNext())
            {
                Type strictType = (Type)types.Current;
                while (true)
                {
                    if (!modules.MoveNext())
                    {
                        // Out of bounds.
                        return false;
                    }

                    Type moduleType = modules.Current.GetType();
                    if (this.IgnoreList.Contains(moduleType))
                    {
                        // Skip ignored type.
                    }
                    else if (strictType == moduleType)
                    {
                        // Type matches.
                        parameters.Add(modules.Current);
                        break;
                    }
                    else
                    {
                        // Type does not match.
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsLeftMatch(IEnumerator modules, IEnumerator types, List<object> parameters)
        {
            while (types.MoveNext())
            {
                Type leftType = (Type)types.Current;
                while (true)
                {
                    if (!modules.MoveNext())
                    {
                        // Out of bounds.
                        return false;
                    }

                    Type moduleType = modules.Current.GetType();
                    if (this.IgnoreList.Contains(moduleType))
                    {
                        // Skip ignored type.
                    }
                    else if (leftType == moduleType)
                    {
                        // Type matches.
                        parameters.Add(modules.Current);
                        break;
                    }
                    else if (moduleType == typeof(StartBranchModule))
                    {
                        // Skip start of a branch.
                    }
                    else if (moduleType == typeof(EndBranchModule))
                    {
                        // Skip complete branch.
                        if (!modules.FindEndOfBranch(typeof(EndBranchModule), typeof(StartBranchModule)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // Type does not match.
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsRightMatch(IEnumerator modules, IEnumerator types, List<object> parameters)
        {
            while (types.MoveNext())
            {
                Type rightType = (Type)types.Current;
                while (true)
                {
                    if (!modules.MoveNext())
                    {
                        // Out of bounds.
                        return false;
                    }

                    Type moduleType = modules.Current.GetType();
                    if (this.IgnoreList.Contains(moduleType))
                    {
                        // Skip ignored type.
                    }
                    else if (rightType == moduleType)
                    {
                        // Type matches.
                        parameters.Add(modules.Current);
                        break;
                    }
                    else if (moduleType == typeof(StartBranchModule))
                    {
                        // Skip complete branch.
                        if (!modules.FindEndOfBranch(typeof(StartBranchModule), typeof(EndBranchModule)))
                        {
                            return false;
                        }
                    }
                    else if (rightType == typeof(EndBranchModule))
                    {
                        // Iterate until end of branch.
                        if (!modules.FindEndOfBranch(typeof(StartBranchModule), typeof(EndBranchModule)))
                        {
                            return false;
                        }

                        // Type is used, goto next type in "types" list.
                        parameters.Add(modules.Current);
                        break;
                    }                    
                    else
                    {
                        // Type does not match.
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
