// reference "System.Core.dll"
// reference "LSystems.dll"

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
        

        System.Random rand = new System.Random();

        [LSystems.Production]
        public object lRule(l p)
        {
            if (rand.Next(2) == 0)
                return parser.Produce("l-rF-");
            else
                return parser.Produce("l+rF+");
        }

        [LSystems.Production]
        public object rRule(r p)
        {
            return parser.Produce("+Fl+r");
        }

        public object Axiom
        {
            get { return Produce(LineColor(0, 1, 0), LineThickness(10), F(), new l()); }
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
