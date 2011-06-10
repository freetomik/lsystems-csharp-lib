using System;
using System.Collections.Generic;

namespace LSystems.Turtle
{
    public class SystemDefintion : LSystems.SystemDefinition
    {
        protected object F() { return new F(); }
        protected object F(double distance) { return new F(distance); }

        protected object f() { return new f(); }
        protected object f(double distance) { return new f(distance); }

        protected object R() { return new R(); }
        protected object R(double angle) { return new R(angle); }

        protected object L() { return new L(); }
        protected object L(double angle) { return new L(angle); }

        protected object LineColor(double r, double g, double b) { return new LineColor(r, g, b); }
        protected object LineThickness(double thickness) { return new LineThickness(thickness); }

        protected object MoveTo(double x, double y) { return new MoveTo(x, y); }
        protected object MoveToRel(double x, double y) { return new MoveToRel(x, y); }        
    }
}