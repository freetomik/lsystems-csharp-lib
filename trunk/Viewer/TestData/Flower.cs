// reference "System.Core.dll"
// reference "LSystems.dll"


namespace Viewer.TestData
{
    class Flower: LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        
        public class P { } // plant
        public class I { } // internode
        public class L { } // leaf       
        public class S { } // seg
        public class D { } // pedicel
        public class W { } // wedge
        public class R { } // flower
        public class ThicknessDown { }
        public class ThicknessUp { }

        public class Yellow : LSystems.Turtle.LineColor 
        {
            public Yellow() : base(1, 1, 0.1) {} 
        }

        public class Green : LSystems.Turtle.LineColor
        {
            public Green() : base(0.2, 0.9, 0.1) { }
        }

        public class White : LSystems.Turtle.LineColor
        {
            public White() : base(1, 0.5, 0.1) { }
        }
        
        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser();

        public Flower()
        {
            parser.Register(typeof(P));
            parser.Register(typeof(I));
            parser.Register(typeof(L));
            parser.Register(typeof(S));
            parser.Register(typeof(D));
            parser.Register(typeof(W));
            parser.Register(typeof(R));

            parser.Register('!', typeof(ThicknessDown));
            parser.Register('?', typeof(ThicknessUp));

            parser.Register('y', typeof(Yellow));
            parser.Register('w', typeof(White));
            parser.Register('g', typeof(Green));
        }        

        [LSystems.Production]        
        public object Produce(P p) { return parser.Produce("I+[+!PR]--//[--L]I[++L][!PR]++//!PR"); }

        [LSystems.Production]
        public object Produce(I i)
        {
            return parser.Produce("FS[//&&L][//^^L]FS");
        }

        [LSystems.Production]
        public object Produce(S s) { return parser.Produce("SFS"); }
        
        [LSystems.Production]
        public object Produce(L l) { return parser.Produce("[g{+f-ff-f+|+f-ff-f}]"); }

        [LSystems.Production]
        public object Produce(R r) { return parser.Produce("[&&&D/W////W////W////W////W]"); }
        
        [LSystems.Production]
        public object Produce(D d) { return parser.Produce("FF"); }

        [LSystems.Production]
        public object Produce(W w) { return parser.Produce("[y?^F][w{&&&&--ff++ff|--ff++ff}]"); }

        [LSystems.Turtle.Interpret]
        public void ReduceThickness(ThicknessDown td)
        {
            Turtle.Thickness = 0.5 * Turtle.Thickness;
        }

        [LSystems.Turtle.Interpret]
        public void ReduceThickness(ThicknessUp tu)
        {
            Turtle.Thickness = 2 * Turtle.Thickness;
        }

        [LSystems.Turtle.Interpret]
        public void Draw(Green c) { Turtle.SetColor(c.Value.R, c.Value.G, c.Value.B); }

        [LSystems.Turtle.Interpret]
        public void Draw(Yellow c) { Turtle.SetColor(c.Value.R, c.Value.G, c.Value.B); }

        [LSystems.Turtle.Interpret]
        public void Draw(White c) { Turtle.SetColor(c.Value.R, c.Value.G, c.Value.B); }
        
        #region IRewriteRules Member

        public object Axiom
        {
            get
            {
                return Produce(
                    new Green(),
                    new LSystems.Turtle.LineThickness(10),
                    new LSystems.Turtle.Distance(10),
                    new LSystems.Turtle.TurnLeft(90), 
                    new LSystems.Turtle.Angle(18),
                    new P());
            }
        }

        public int Depth
        {
            get { return 5; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        #endregion
    }
}
