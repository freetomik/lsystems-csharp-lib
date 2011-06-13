using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public class i { }
    public class a { }

    public class Leaf : LSystems.Turtle.SystemDefintion
    {
        public double Angle { get; set; }
        public double Distance { get; set; }

        public Leaf()
        {
            this.Angle = 60;
            this.Distance = 10;
        }

        public override object Axiom
        {
            get { return Produce(L(90), new i()); }
        }

        public override int Depth
        {
            get { return 8; }
        }

        [LSystems.Production]
        public object Branch(i m)
        {
            return Produce(
                new a(),
                StartBranch, R(this.Angle), new i(), EndBranch,
                StartBranch, L(this.Angle), new i(), EndBranch,
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
            this.Turtle.LineParameters(3, 0, 0.7, 0);
            this.Turtle.Forward(this.Distance, true);
        }

        [LSystems.Turtle.Interpret]
        public void DrawI(i m)
        {
            this.Turtle.LineParameters(1, 1, 0, 0);
            this.Turtle.Forward(this.Distance, true);
        }
    }
}
