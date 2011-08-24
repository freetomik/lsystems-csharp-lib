using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystems.Turtle
{
    public class Module<T>
    {
        public T Value { get; protected set; }

        protected Module(T value)
        {
            this.Value = value;
        }

        protected Module()
        {
        }
    }

    public class F : Module<double>
    {
        public const char Letter = 'F';
        public F(double distance) : base(distance) { }
        public F() { }    
    }

    public class f : Module<double>
    {
        public const char Letter = 'f';
        public f(double distance) : base(distance) { }
        public f() { }
    }

    public class TurnLeft : Module<double>
    {
        public const char Letter = '+';
        public TurnLeft(double angle) : base(angle) { }
        public TurnLeft() { }        
    }

    public class TurnRight : Module<double>
    {
        public const char Letter = '-';
        public TurnRight(double angle) : base(angle) { }
        public TurnRight() { }        
    }

    public class PitchUp : Module<double>
    {
        public const char Letter = '^';
        public PitchUp(double angle) : base(angle) { }
        public PitchUp() { }
    }

    public class PitchDown : Module<double>
    {
        public const char Letter = '&';
        public PitchDown(double angle) : base(angle) { }
        public PitchDown() { }
    }

    public class RollLeft : Module<double>
    {
        public const char Letter = '\\';
        public RollLeft(double angle) : base(angle) { }
        public RollLeft() { }
    }

    public class RollRight : Module<double>
    {
        public const char Letter = '/';
        public RollRight(double angle) : base(angle) { }
        public RollRight() { }
    }

    public class TurnAround
    {
        public const char Letter = '|';
    }

    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class MoveTo : Module<Point>
    {
        public MoveTo(double x, double y) : base(new Point() { X = x, Y = y }) { }
        public MoveTo() { }        
    }

    public class MoveToRel : Module<Point>
    {
        public MoveToRel(double x, double y) : base(new Point() { X = x, Y = y }) { }
        public MoveToRel() { }        
    }

    public struct Color
    {
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }
    }

    public class LineColor : Module<Color>
    {
        public LineColor(double r, double g, double b)
            : base(new Color() { R = r, G = g, B = b }) 
        {
        }
        public LineColor() { }        
    }

    public class LineThickness : Module<double>
    {
        public LineThickness(double thickness) : base(thickness) { }
        public LineThickness() { }
    }

    public class SurfaceBegin
    {
        public const char Letter = '{';
    }

    public class SurfaceEnd
    {
        public const char Letter = '}';
    }
}
