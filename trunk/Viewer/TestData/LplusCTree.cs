// This is an example from L+C description.
// http://algorithmicbotany.org/papers/l+c.tcs2003.pdf

using System;
using LSystems;
using System.Collections;

namespace SystemDefinitionsTest
{
    public class Axis
    {
        public double Length { get; private set; }
        public double Angle { get; private set; }
        public Axis(double l, double a)
        {
            this.Length = l;
            this.Angle = a;
        }
    }

    public class Metamer
    {
        public double Angle { get; private set; }
        public Metamer(double a)
        {            
            this.Angle = a;
        }
    }

    public class Internode
    {
        public double Length { get; private set; }
        public double Area { get; private set; }
        public Internode(double l, double a)
        {
            this.Length = l;
            this.Area = a;
        }
    }

    public class LplusCTree : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        public const double Delay = 1;
        public const double BranchingAngle = 45;
        public const double LengthGrowRate = 1.33;

        public object Axiom
        {
            get
            {
                return Produce(TurnLeft(90), new Axis(0, BranchingAngle));
            }
        }

        public int Depth
        {
            get { return 17; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.RightToLeft; }
        }
        
        [LSystems.Production]        
        public object ApexGrow(Axis a)
        {
            if (a.Length < 0)
                return new Axis(a.Length + 1, a.Angle);
            else
                return Produce(new Metamer(a.Angle), new Axis(0, -a.Angle));
        }
        
        [LSystems.Production("i >> sb i2 eb i3")]
        [LSystems.Ignore(new[] { typeof(LSystems.Turtle.TurnRight) })]
        public object InternodeGrow1(Internode i, StartBranchModule sb, Internode i2, EndBranchModule eb, Internode i3)
        {
            return new Internode(i.Length * LengthGrowRate, i2.Area + i3.Area);
        }

        [LSystems.Production("i >> i2")]
        [LSystems.Ignore(new[] { typeof(LSystems.Turtle.TurnRight) })]
        public object InternodeGrow2(Internode i, Internode i2)
        {
            return new Internode(i.Length * LengthGrowRate, i2.Area);
        }

        [LSystems.Decomposition]
        public object MetamerGrow(Metamer meta)
        {
            return Produce(
                new Internode(1, 1),
                StartBranch, 
                TurnRight(meta.Angle), new Axis(-Delay, meta.Angle),
                EndBranch,
                new Internode(1, 1));
        }
        
        [LSystems.Turtle.Interpret]
        public void DrawInternode(Internode i)
        {
            if (i.Length < 1) Turtle.SetColor(0, 1, 0);
            else if (i.Length < 3) Turtle.SetColor(0, 0.5, 0);
            else Turtle.SetColor(0.2, 0.1, 0);
            Turtle.SetThickness(Math.Pow(i.Area, 0.5));
            Turtle.Forward(i.Length, true);
        }        
    }
};
