using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.TestData
{
    public class ASurfaceTest : LSystems.Turtle.SystemDefinition, LSystems.IRewriteRules
    {
        private LSystems.Turtle.StringParser parser = new LSystems.Turtle.StringParser();       

        [LSystems.Production]
        public object Rule(LSystems.Turtle.f f)
        {
            return parser.Produce("f+f-f-f+f");
        }        

        public object Axiom
        {
            get { return parser.Produce("{f}"); }
        }

        public int Depth
        {
            get { return 4; }
        }

        public LSystems.RewriteDirection RewriteDirection
        {
            get { return LSystems.RewriteDirection.LeftToRight; }
        }
    }

}
