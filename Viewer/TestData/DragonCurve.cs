#pragma reference "System.Core.dll"
#pragma reference "LSystems.dll"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public class DragonCurve : LSystems.Turtle.SystemDefintion, LSystems.IRewriteRules
    {
        public class l { };
        public class r { };

        public DragonCurve()
        {
            this.Angle = 90;
            this.Distance = 10;
        }

        public double Angle { get; set; }
        public double Distance { get; set; }

        [LSystems.Production]
        public object lRule(l p)
        {
            return Produce(
                new l(),
                R(this.Angle),
                new r(),
                F(this.Distance),
                R(this.Angle));
        }

        [LSystems.Production]
        public object rRule(r p)
        {
            return Produce(
                L(this.Angle),
                F(this.Distance),
                new l(),
                L(this.Angle),
                new r());
        }

        public object Axiom
        {
            get { return Produce(F(), new l()); }
        }

        public int Depth
        {
            get { return 10; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }
}
