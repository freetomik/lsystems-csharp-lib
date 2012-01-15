// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData 
{
    public class DragonCurve : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class l { };
        public class r { };

        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser();

        public DragonCurve()
        {
            parser.Register(typeof(l));
            parser.Register(typeof(r));            
        }

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
            get { return Produce(LineColor(0, 1, 0), F(), new l()); }
        }

        public int Depth
        {
            get { return 2; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }
}
