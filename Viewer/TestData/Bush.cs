using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    class Bush : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class A { }
        public class S { }
        public class L { }        
        
        private LSystems.Turtle.StringParser parser = 
            new LSystems.Turtle.StringParser()
        {
            Angle = 22.5,            
            CharToObject = c =>
            {
                switch (c)
                {
                    case 'A': return new A();
                    case 'S': return new S();
                    case 'L': return new L();
                    case 'g': return new LSystems.Turtle.LineColor(0.2, 0.9, 0.1);
                    case 'b': return new LSystems.Turtle.LineColor(0.2, 0.2, 0.1);
                    default: return null;
                }
            }
        };

        [LSystems.Production]
        public object Produce(A a)
        {
            return parser.Produce("b[&FLA]/////b[&FLA]///////b[&FLA]");
        }

        [LSystems.Production]
        public object Produce(LSystems.Turtle.F f)
        {
            return parser.Produce("S/////F");
        }

        [LSystems.Production]
        public object Produce(S s)
        {
            return parser.Produce("FL");
        }

        [LSystems.Production]
        public object Produce(L l)
        {
            return parser.Produce("[g^^{-f+f+f-|-f+f+f}]");
        }

        #region IRewriteRules Member

        public object Axiom
        {
            get
            {
                return Produce(new LSystems.Turtle.L(90), new A());
            }
        }

        public int Depth
        {
            get { return 6; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        #endregion
    }
}
