﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        
        private LSystems.Turtle.StringParser parser = 
            new LSystems.Turtle.StringParser()
        {
            Angle = 18,            
            CharToObject = c =>
            {
                switch (c)
                {
                    case 'P': return new P();
                    case 'I': return new I();
                    case 'L': return new L();
                    case 'S': return new S();
                    case 'D': return new D();
                    case 'W': return new W();
                    case 'R': return new R();
                    case 'w': return new LSystems.Turtle.LineColor(1, 1, 0.1);
                    case 'g': return new LSystems.Turtle.LineColor(0.2, 0.9, 0.1);                    
                    default: return null;
                }
            }
        };

        [LSystems.Production]        
        public object Produce(P p) { return parser.Produce("I+[+PR]--//[--L]I[++L][PR]++//PR"); }

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
        public object Produce(W w) { return parser.Produce("[^F]w[{&&&&--ff++ff|--ff++ff}]"); }

        [LSystems.Turtle.Interpret]
        public void Draw(LSystems.Turtle.F f)
        {
            this.Turtle.SetThicknessAndColor(10, 0, 0.7, 0);
            this.Turtle.Forward(100, true);
        }
        
        #region IRewriteRules Member

        public object Axiom
        {
            get
            {
                return Produce(new LSystems.Turtle.L(90), new P());
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
