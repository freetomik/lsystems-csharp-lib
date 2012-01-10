// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData
{
    class Hilbert3d : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class A {}
        public class B {}
        public class C {}
        public class D {}

        private LSystems.Turtle.StringParser parser = 
            new LSystems.Turtle.StringParser()
        {
            CharToObject = (c, p) =>
            {
                switch (c)
                {
                    case 'A': return new A();
                    case 'B': return new B();
                    case 'C': return new C();
                    case 'D': return new D();
                    default: return null;
                }
            }
        };

        [LSystems.Production]
        public object Produce(A a)
        {
            return parser.Produce("B-F+CFC+F-D&F^D-F+&&CFC+F+B//");
        }

        [LSystems.Production]
        public object Produce(B a)
        {
            return parser.Produce("A&F^CFB^F^D^^-F-D^|F^B|FC^F^A//");
        }

        [LSystems.Production]
        public object Produce(C c)
        {
            return parser.Produce("|D^|F^B-F+C^F^A&&FA&F^C+F+B^F^D//");
        }

        [LSystems.Production]
        public object Produce(D d)
        {
            return parser.Produce("|CFB-F+B|FA&F^A&&FB-F+B|FC//");
        }

        double color = 0;
        double delta = 0.05;

        [LSystems.Turtle.Interpret]
        public void Draw(LSystems.Turtle.F f)
        {
            this.Turtle.Thickness = 33;
            this.Turtle.SetColor(1, color, 0);            
            this.Turtle.Forward(100, true);

            color += delta;
            if (color > 1)
            {
                color = 1;
                delta = -delta;
            }
            else if (color < 0)
            {
                color = 0;
                delta = -delta;
            }
        }

        #region IRewriteRules Member

        public object Axiom
        {
            get 
            { 
                return new A(); 
            }
        }

        public int Depth
        {
            get { return 4; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        #endregion
    }
}
