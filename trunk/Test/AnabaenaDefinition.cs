using System;
using System.Collections;

namespace Test
{
    public class W
    {
        public double T { get; private set; }
        public W(double t)
        {
            this.T = t;
        }
        public override string ToString()
        {
            return string.Format("W({0:0.00})", this.T);
        }
    }

    public class M
    {
        public double T { get; private set; }
        public M(double t)
        {
            this.T = t;
        }
        public override string ToString()
        {
            return string.Format("M({0:0.00})", this.T);
        }
    }

    public class AnabaenaDefinition : LSystems.SystemDefinition
    {
        public const double Div = 1;
        public const double ShortCell = 0;
        public const double LongCell = 0.2;
        public const double TimeStep = 0.7;
        public const double Epsilon = 0.000001;

        [LSystems.Production("w")]
        public object WallGrow(W w)
        {
            return new W(w.T + TimeStep);
        }

        [LSystems.Production("m")]
        public object CellGrow(M m)
        {
            if (m.T < Div - Epsilon)
                return new M(m.T + TimeStep);
            else
                return null;
        }

        [LSystems.Production("leftWall < m > rightWall")]
        public object Divide(W leftWall, M m, W rightWall)
        {
            if (m.T > Div - Epsilon)
            {
                if (leftWall.T < rightWall.T)
                {
                    return Produce(
                        new M(m.T - Div + LongCell),
                        new W(m.T - Div),
                        new M(m.T - Div + ShortCell));
                }
                else
                {
                    return Produce(
                        new M(m.T - Div + ShortCell),
                        new W(m.T - Div),
                        new M(m.T - Div + LongCell));
                }
            }
            return null;
        }

        [LSystems.Decomposition]
        public object One(M m)
        {
            return new object[] { new W(m.T), new W(m.T) };
        }

        [LSystems.Decomposition]
        public object Two(W m)
        {
            return new object[] { new M(m.T), new M(m.T) };
        }

        public static void DoTest()
        {
            // Create L-System.
            var definition = new AnabaenaDefinition();
            LSystems.System system = LSystems.SystemBuilder.BuildSystem(definition);
            system.String = new object[] 
            {
                new W(0), new M(0), new W(AnabaenaDefinition.TimeStep)
            };

            // Rewrite and printout the result.
            Print(system.String as IEnumerable);

            for (int i = 0; i < 7; ++i)
            {
                system.RewriteLeftToRight();
                //system.Decomposite();
                Print(system.String as IEnumerable);
            }
        }

        private static void Print(IEnumerable modules)
        {
            foreach (object module in modules)
            {
                Console.Write("{0} ", module);
            }
            Console.WriteLine();
            Console.WriteLine("---");
        }
    }
}
