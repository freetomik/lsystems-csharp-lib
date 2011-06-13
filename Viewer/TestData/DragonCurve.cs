﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public class DragonCurve : LSystems.Turtle.SystemDefintion
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

        public override object Axiom
        {
            get { return Produce(F(), new l()); }
        }

        public override int Depth
        {
            get { return 10; }
        }
    }
}
