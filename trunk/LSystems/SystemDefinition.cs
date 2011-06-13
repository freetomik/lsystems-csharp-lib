using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystems
{
    public class SystemDefinition
    {
        protected object Empty
        {
            get { return new EmptyModule(); }
        }

        protected object StartBranch
        {
            get { return new StartBranchModule(); }
        }

        protected object EndBranch
        {
            get { return new EndBranchModule(); }
        }

        protected object Produce(params object[] modules)
        {
            return modules.ToList();
        }

        public virtual object Axiom
        {
            get { return null; }
        }

        public virtual int Depth
        {
            get { return 1; }
        }
    }
}
