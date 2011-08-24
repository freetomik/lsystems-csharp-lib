using System;
using System.Collections.Generic;

namespace LSystems.Turtle
{
    public class SystemDefinition : LSystems.SystemDefinition
    {
        protected object F() { return new F(); }
        protected object F(double distance) { return new F(distance); }

        protected object f() { return new f(); }
        protected object f(double distance) { return new f(distance); }

        protected object TurnRight() { return new TurnRight(); }
        protected object TurnRight(double angle) { return new TurnRight(angle); }

        protected object TurnLeft() { return new TurnLeft(); }
        protected object TurnLeft(double angle) { return new TurnLeft(angle); }

        protected object PitchUp() { return new PitchUp(); }
        protected object PitchUp(double angle) { return new PitchUp(angle); }

        protected object PitchDown() { return new PitchDown(); }
        protected object PitchDown(double angle) { return new PitchDown(angle); }

        protected object RollLeft() { return new RollLeft(); }
        protected object RollLeft(double angle) { return new RollLeft(angle); }

        protected object RollRight() { return new RollRight(); }
        protected object RollRight(double angle) { return new RollRight(angle); }

        protected object LineColor(double r, double g, double b) { return new LineColor(r, g, b); }
        protected object LineThickness(double thickness) { return new LineThickness(thickness); }

        protected object MoveTo(double x, double y) { return new MoveTo(x, y); }
        protected object MoveToRel(double x, double y) { return new MoveToRel(x, y); }

        public ITurtle Turtle { get; set; }        
    }
}