using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections;
using System.Windows;

namespace Viewer.View
{
    public class Turtle2d : LSystems.Turtle.ITurtle
    {
        private DrawingContext dc;
        private Stack<Pair<Point, double>> stack;
        private Pen pen;

        private Turtle2d(DrawingContext dc)
        {
            this.dc = dc;
            this.stack = new Stack<Pair<Point, double>>();
            this.pen = new Pen(Brushes.Black, 1);

            this.Push();
        }

        public Rect Bounds 
        { 
            get; 
            protected set; 
        }

        private Point Position
        {
            get { return stack.Peek().First; }
            set 
            {
                stack.Peek().First = value;

                Rect b = this.Bounds;
                b.Union(value);
                this.Bounds = b;
            }
        }

        private double Angle
        {
            get { return stack.Peek().Second; }
            set { stack.Peek().Second = value; }
        }        

        public void Push()
        {
            if (stack.Count > 0)
            {
                stack.Push(new Pair<Point, double>(Position, Angle));
            }
            else
            {
                stack.Push(new Pair<Point, double>(new Point(0, 0), 0));
            }
        }

        public void Pop()
        {
            stack.Pop();
        }

        public void Forward(double distance, bool drawLine)
        {
            Point newPosition = this.Position;

            newPosition.Offset(
                distance * Math.Cos(this.Angle * Math.PI / 180.0),
                distance * Math.Sin(this.Angle * Math.PI / 180.0));

            if (drawLine)
            {
                this.dc.DrawLine(this.pen, this.Position, newPosition);
            }

            this.Position = newPosition;
        }

        public void Move(double x, double y)
        {
            this.Position = new Point(x, y);
        }

        public void MoveRel(double x, double y)
        {
            Point newPosition = this.Position;

            newPosition.Offset(x, y);

            this.Position = newPosition;
        }

        public void Turn(double u)
        {
            this.Angle += u;
        }

        public void LineThickness(double thickness)
        {
            this.pen = new Pen(this.pen.Brush, thickness);
        }

        public void LineColor(double r, double g, double b)
        {
            this.pen = new Pen(
                new SolidColorBrush(Color.FromRgb(
                (byte)(255 * r),
                (byte)(255 * g),
                (byte)(255 * b))), this.pen.Thickness);
        }

        public void LineParameters(double thickness, double r, double g, double b)
        {
            Color color = Color.FromRgb(
                (byte)(255 * r),
                (byte)(255 * g),
                (byte)(255 * b));
            if ((this.pen.Brush as SolidColorBrush).Color != color
                || this.pen.Thickness != thickness)
            {
                this.pen = new Pen(new SolidColorBrush(color), thickness);
            }            
        }

        private class Pair<T, U>
        {
            public Pair()
            {
            }

            public Pair(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }
            public U Second { get; set; }
        }

        public static void Render(object system, DrawingContext dc, out Rect boundingRect)
        {
            var turtle = new Turtle2d(dc);

            LSystems.System lSystem = (LSystems.System)system;

            if (lSystem.Definition is LSystems.Turtle.SystemDefintion)
            {
                ((LSystems.Turtle.SystemDefintion)lSystem.Definition).Turtle = turtle;
            }

            

            var interpreter = new LSystems.Turtle.Interpreter();

            interpreter.RegisterExternalFunctions(lSystem.Definition);

            interpreter.Interpret(turtle, lSystem.String as IEnumerable);

            if (lSystem.Definition is LSystems.Turtle.SystemDefintion)
            {
                ((LSystems.Turtle.SystemDefintion)lSystem.Definition).Turtle = null;
            }

            boundingRect = turtle.Bounds;
        }
    }
}
