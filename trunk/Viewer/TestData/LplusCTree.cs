// This is an example from L+C description, strange,
// but InternodeGrow3 is never called.

using System;
using LSystems;
using System.Collections;

namespace SystemDefinitionsTest
{
    public class A
    {
        public double Length { get; private set; }
        public double Angle { get; private set; }
        public A(double l, double a)
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

    public class LplusCTree : LSystems.Turtle.SystemDefintion
    {
        public const double Delay = 1;
        public const double BranchingAngle = 45;
        public const double LengthGrowRate = 1.33;

        public override object Axiom
        {
            get
            {
                return Produce(L(90), new A(0, BranchingAngle));
            }
        }

        public override int Depth
        {
            get { return 17; }
        }
        
        [LSystems.Production]        
        public object ApexGrow(A a)
        {
            if (a.Length < 0)
                return new A(a.Length + 1, a.Angle);
            else
                return Produce(new Metamer(a.Angle), new A(0, -a.Angle));
        }
        
        [LSystems.Production("i >> sb i2 eb i3")]
        [LSystems.Ignore(new[] { typeof(LSystems.Turtle.R) })]
        public object InternodeGrow1(Internode i, StartBranchModule sb, Internode i2, EndBranchModule eb, Internode i3)
        {
            return new Internode(i.Length * LengthGrowRate, i2.Area + i3.Area);
        }

        [LSystems.Production("i >> i2")]
        [LSystems.Ignore(new[] { typeof(LSystems.Turtle.R) })]
        public object InternodeGrow2(Internode i, Internode i2)
        {
            return new Internode(i.Length * LengthGrowRate, i2.Area);
        }

        [LSystems.Production("i >> a")]
        [LSystems.Ignore(new [] { typeof(LSystems.Turtle.R) })]
        public object InternodeGrow3(Internode i, A a)
        {
            return new Internode(i.Length * LengthGrowRate, i.Area);
        }

        [LSystems.Decomposition]
        public object MetamerGrow(Metamer meta)
        {
            return Produce(
                new Internode(1, 1),
                StartBranch,
                R(meta.Angle),
                new A(-Delay, meta.Angle),
                EndBranch,
                new Internode(1, 1));
        }
        
        [LSystems.Turtle.Interpret]
        public void DrawInternode(Internode i)
        {
            Turtle.LineThickness(Math.Pow(i.Area, 0.5));
            Turtle.Forward(i.Length, true);
        }        
    }
};
