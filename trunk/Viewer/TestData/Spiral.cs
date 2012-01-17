// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData
{
    public class Spiral : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public class e
        {
            public int Index { get; set; }
            public e() { this.Index = 0; }
            public e(double index) { this.Index = (int)index; }
        };

        public class s
        {
            public int Index { get; set; }
            public s() { this.Index = 0; }
            public s(double index) { this.Index = (int)index; }
        };

        public class D { };

        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser();

        public Spiral()
        {
            parser.Register(typeof(e));
            parser.Register(typeof(s));
            parser.Register(typeof(D));
        }

        private const int GrowIndex = 8;

        [LSystems.Production]
        public object Grow(e endpoint)
        {
            int index = (endpoint.Index + 1) % GrowIndex;
            return parser.Produce(string.Format(
                "^+DF{0}e({1})",
                (index == 0 ? "[Ds]" : ""),
                index));
        }

        [LSystems.Production]
        public object Grow(s endpoint)
        {
            int index = (endpoint.Index + 1) % GrowIndex;
            return parser.Produce(string.Format(
                "-&DF{0}s({1})",
                (index == 0 ? "[De]" : ""),
                index));
        }

        [LSystems.Turtle.Interpret]
        public void Smaller(D d)
        {
            Turtle.SetColor(1, 1.0 - Turtle.Distance / 100.0, 0);
            Turtle.Distance = Turtle.Distance * 0.95;
            Turtle.Thickness = Turtle.Thickness * 0.93;
        }

        public object Axiom
        {
            get { return parser.Produce("#(100)d(100)a(45)\\a(20)e"); }
        }

        public int Depth
        {
            get { return 80; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }
}
