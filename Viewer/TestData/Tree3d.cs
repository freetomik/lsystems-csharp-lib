using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    class Tree3d : LSystems.Turtle.SystemDefintion, LSystems.IRewriteRules
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

        private const double Alpha1 = 30;
        private const double Alpha2 = -30;
        private const double Phi1 = 137;
        private const double Phi2 = 137;
        private const double R1 = 0.8;
        private const double R2 = 0.8;
        private const double Q = 0.50;
        private const double E = 0.50;
        private const double W0 = 30;

        [LSystems.Production]
        public object Grow(A a)
        {
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
            get { return 10; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }

        #endregion
    }
}
