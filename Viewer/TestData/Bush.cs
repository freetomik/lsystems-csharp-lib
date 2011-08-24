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
        public class ThicknessDown { }
        
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
                    case '!': return new ThicknessDown();
                    case 'g': return new LSystems.Turtle.LineColor(0.2, 0.9, 0.1);                    
                    default: return null;
                }
            }
        };

        [LSystems.Production]
        public object Produce(A a)
        {
            return parser.Produce("[&FL!A]/////[&FL!A]///////[&FL!A]");
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

        [LSystems.Turtle.Interpret]
        public void ReduceThickness(ThicknessDown td)
        {
            Turtle.SetThickness(0.5 * Turtle.GetThickness());
        }

        #region IRewriteRules Member

        public object Axiom
        {
            get
            {
                return Produce(
                    new LSystems.Turtle.LineThickness(30), 
                    new LSystems.Turtle.LineColor(0.2, 0.2, 0.1), 
                    new LSystems.Turtle.TurnLeft(90), 
                    new A());
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
