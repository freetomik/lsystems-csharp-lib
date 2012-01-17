// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData
{
    class Bush : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class A { }
        public class S { }
        public class L { }
        public class Green { }
        public class ThicknessDown { }

        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser();

        public Bush()
        {
            parser.Register(typeof(A));
            parser.Register(typeof(S));
            parser.Register(typeof(L));
            parser.Register('!', typeof(ThicknessDown));
            parser.Register('g', typeof(Green));
        }        

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
            Turtle.Thickness = 0.5 * Turtle.Thickness;
        }

        [LSystems.Turtle.Interpret]
        public void DrawGreen(Green g)
        {
            Turtle.SetColor(0.2, 0.9, 0.1);
        }        

        #region IRewriteRules Member

        public object Axiom
        {
            get
            {
                return Produce(
                    new LSystems.Turtle.LineThickness(30),
                    new LSystems.Turtle.Distance(100),
                    new LSystems.Turtle.Angle(22.5), 
                    new LSystems.Turtle.LineColor(0.9, 0.6, 0.1), 
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
