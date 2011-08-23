using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Viewer.View
{
    public class Turtle2d : LSystems.Turtle.ITurtle
    {
        private DrawingContext dc;
        private Stack<Pair<Point, Matrix3D>> stack;
        private Pen pen;
        private List<Point> surfacePoints = new List<Point>();
        private bool isSurfaceMode;

        private Turtle2d(DrawingContext dc)
        {
            this.dc = dc;
            this.stack = new Stack<Pair<Point, Matrix3D>>();
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

        private Matrix3D Rotation
        {
            get { return stack.Peek().Second; }
            set { stack.Peek().Second = value; }
        }

        public void Push()
        {
            if (stack.Count > 0)
            {
                stack.Push(new Pair<Point, Matrix3D>(Position, Rotation));
            }
            else
            {
                stack.Push(new Pair<Point, Matrix3D>(new Point(0, 0), Matrix3D.Identity));
            }
        }

        public void Pop()
        {
            stack.Pop();
        }

        public void Forward(double distance, bool drawLine)
        {
            var offset = Rotation.Transform(new Point3D(distance, 0, 0));

            var newPosition = this.Position;
            newPosition.Offset(offset.X, offset.Y);

            if (drawLine)
            {
                this.dc.DrawLine(this.pen, this.Position, newPosition);
            }

            if (this.isSurfaceMode)
            {
                this.surfacePoints.Add(newPosition);
            }

            this.Position = newPosition;
        }

        public void Move(double x, double y, double z)
        {
            this.Position = new Point(x, y);
        }

        public void MoveRel(double x, double y, double z)
        {
            Point newPosition = this.Position;

            newPosition.Offset(x, y);

            this.Position = newPosition;
        }

        public void Turn(double angle)
        {
            this.Rotation = Matricies.TurnMartix(angle) * this.Rotation;
        }

        public void Pitch(double angle)
        {
            this.Rotation = Matricies.PitchMartix(angle) * this.Rotation;
        }

        public void Roll(double angle)
        {
            this.Rotation = Matricies.RollMartix(angle) * this.Rotation;
        }

        public void SetThickness(double thickness)
        {
            this.pen = new Pen(this.pen.Brush, thickness);
        }

        public void SetColor(double r, double g, double b)
        {
            this.pen = new Pen(
                new SolidColorBrush(Color.FromRgb(
                (byte)(255 * r),
                (byte)(255 * g),
                (byte)(255 * b))), this.pen.Thickness);
        }

        public void SetThicknessAndColor(double thickness, double r, double g, double b)
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

        public void SurfaceBegin()
        {
            this.isSurfaceMode = true;
            this.surfacePoints.Add(this.Position);
        }

        public void SurfaceEnd()
        {
            if (this.surfacePoints.Count > 0)
            {            
                PathFigure figure = new PathFigure();
                figure.StartPoint = this.surfacePoints[0];

                PathSegmentCollection segments = new PathSegmentCollection();
                for (int i = 1; i < this.surfacePoints.Count; i++)
                    segments.Add(new LineSegment(this.surfacePoints[i], true));

                figure.Segments = segments;
                
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);

                this.dc.DrawGeometry(this.pen.Brush, this.pen, geometry);

                this.surfacePoints.Clear();
            }

            this.isSurfaceMode = false;
            
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

            if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
            {
                ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = turtle;
            }

            var interpreter = new LSystems.Turtle.Interpreter();

            interpreter.RegisterExternalFunctions(lSystem.Definition);

            interpreter.Interpret(turtle, lSystem.String as IEnumerable);

            if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
            {
                ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = null;
            }

            boundingRect = turtle.Bounds;
        }
    }
}
