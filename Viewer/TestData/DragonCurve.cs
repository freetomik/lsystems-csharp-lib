#pragma reference "System.Core.dll"
#pragma reference "LSystems.dll"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public class DragonCurve : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class l { };
        public class r { };

        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser()
        {
            CharToObject = c =>
            {
                switch (c)
                {
                    case 'l': return new l();
                    case 'r': return new r();
                    default: return null;
                }
            }
        };
        

        [LSystems.Production]
        public object lRule(l p)
        {
            return parser.Produce("l-rF-");
        }

        [LSystems.Production]
        public object rRule(r p)
        {
            return parser.Produce("+Fl+r");
        }

        public object Axiom
        {
            get { return parser.Produce("Fl"); }
        }

        public int Depth
        {
            get { return 10; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }
}
