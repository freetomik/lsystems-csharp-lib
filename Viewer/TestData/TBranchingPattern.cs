// reference "System.Core.dll"
// reference "LSystems.dll"

namespace Viewer.TestData
{
    public class TBranchingPattern : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public const double R = 1.456;
        public const double Angle = 85;
        
        public class A 
        {
            public double S { get; set; }
            public A(double s) { this.S = s; }             
        };
  
        [LSystems.Production]
        public object TheRule(A a)
        {
            return Produce(
                LineThickness(a.S / 10),
                F(a.S),
                StartBranch,
                    TurnLeft(Angle),
                    new A(a.S / R),
                EndBranch,
                StartBranch,
                    TurnRight(Angle),
                    new A(a.S / R),
                EndBranch
            );
        }
        
        public object Axiom
        {
            get { return Produce(TurnLeft(90), new A(400)); }
        }

        public int Depth { get { return 12; }  }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }
}
