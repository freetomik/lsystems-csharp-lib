// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData
{
    public class i { }
    public class a { }

    public class Leaf : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public double Angle { get; set; }
        public double Distance { get; set; }

        public Leaf()
        {
            this.Angle = 60;
            this.Distance = 10;
        }

        public object Axiom
        {
            get { return Produce(TurnLeft(90), new i()); }
        }

        public int Depth
        {
            get { return 5; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        [LSystems.Production]
        public object Branch(i m)
        {
            return Produce(
                new a(),
                StartBranch, TurnRight(this.Angle), new i(), EndBranch,
                StartBranch, TurnLeft(this.Angle), new i(), EndBranch,
                new a(), new i());
        }

        [LSystems.Production]
        public object Grow(a m)
        {
            return Produce(new a(), new a());
        }

        [LSystems.Turtle.Interpret]
        public void DrawA(a m)
        {
            this.Turtle.Thickness = 5;
            this.Turtle.SetColor(0, 0.7, 0);
            this.Turtle.Forward(this.Distance, true);
        }

        [LSystems.Turtle.Interpret]
        public void DrawI(i m)
        {
            this.Turtle.Thickness = 1;
            this.Turtle.SetColor(1, 0, 0);
            this.Turtle.Forward(this.Distance, true);
        }
    }
}
