// These threes are taken from "L-sastems: from the Theory to Visual Models of Plants"
// http://algorithmicbotany.org/papers/l-sys.csiro96.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public abstract class Tree3d : LSystems.Turtle.SystemDefintion, LSystems.IRewriteRules
    {
        public class A
        {
            public double Length { get; private set; }
            public double Thickness { get; private set; }
            public A(double length, double thickness)
            {
                this.Thickness = thickness;
                this.Length = length;
            }
        }

        protected double Alpha1 { get; set; }
        protected double Alpha2 { get; set; }
        protected double Phi1 { get; set; }
        protected double Phi2 { get; set; }
        protected double R1 { get; set; }
        protected double R2 { get; set; }
        protected double Q { get; set; }
        protected double E { get; set; }
        protected double W0 { get; set; }
        protected double Min { get; set; }
        protected int N { get; set; }

        [LSystems.Production]
        public object Grow(A a)
        {
            if (a.Length < Min)
                return null;

            return Produce(
                LineThickness(a.Thickness),
                F(a.Length),
                StartBranch,
                    R(Alpha1),
                    new LSystems.Turtle.RollLeft(Phi1),
                    new A(a.Length * R1, a.Thickness * Math.Pow(Q, E)),
                EndBranch,
                StartBranch,
                    R(Alpha2),
                    new LSystems.Turtle.RollLeft(Phi2),
                    new A(a.Length * R2, a.Thickness * Math.Pow(1.0 - Q, E)),
                EndBranch
                );
        }

        #region IRewriteRules Member

        public object Axiom
        {
            get { return Produce(LineColor(0.7, 0.5, 0), L(90), new A(100, W0)); }
        }

        public int Depth
        {
            get { return this.N; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        #endregion

        private static double [,] parameters = new double[,]
        {
            { .75, .77, 35, -35, 0, 0, 30, .50, .40, 0.0, 10},
            { .65, .71, 27, -68, 0, 0, 20, .53, .50, 1.7, 12},
            { .50, .85, 25, -15, 180, 0, 20, .45, .50, 0.5, 9},
            { .60, .85, 25, -15, 180, 180, 20, .45, .50, 0.0, 10},
            { .58, .83, 30, 15, 0, 180, 20, .40, .50, 1.0, 11},
            { .92, .37, 0, 60, 180, 0, 2, .50, .00, 0.5, 15},
            { .80, .80, 30, -30, 137, 137, 30, .50, .50, 0.0, 10},
            { .95, .75, 5, -30, -90, 90, 40, .60, .45, 25.0, 12},
            { .55, .95, -5, 30, 137, 137, 5, .40, .00, 5.0, 12}
        };

        protected Tree3d(int index)
        {
            this.R1 = parameters[index, 0];
            this.R2 = parameters[index, 1];
            this.Alpha1 = parameters[index, 2];
            this.Alpha2 = parameters[index, 3];
            this.Phi1 = parameters[index, 4];
            this.Phi2 = parameters[index, 5];
            this.W0 = parameters[index, 6];
            this.Q = parameters[index, 7];
            this.E = parameters[index, 8];
            this.Min = parameters[index, 9];
            this.N = (int)parameters[index, 10];
        }
    }

    public class Tree3dA : Tree3d
    {
        public Tree3dA() : base(0) {}
    }

    public class Tree3dB : Tree3d
    {
        public Tree3dB() : base(1) { }
    }

    public class Tree3dC : Tree3d
    {
        public Tree3dC() : base(2) { }
    }

    public class Tree3dD : Tree3d
    {
        public Tree3dD() : base(3) { }
    }

    public class Tree3dE : Tree3d
    {
        public Tree3dE() : base(4) { }
    }

    public class Tree3dF : Tree3d
    {
        public Tree3dF() : base(5) { }
    }

    public class Tree3dG : Tree3d
    {
        public Tree3dG() : base(6) { }
    }

    public class Tree3dH : Tree3d
    {
        public Tree3dH() : base(7) { }
    }

    public class Tree3dI: Tree3d
    {
        public Tree3dI() : base(8) { }
    }
}
