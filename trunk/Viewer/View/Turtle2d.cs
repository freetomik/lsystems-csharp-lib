//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Media;
//using System.Collections;
//using System.Windows;
//using System.Windows.Media.Media3D;

//namespace Viewer.View
//{
//    public class Turtle2d : LSystems.Turtle.ITurtle
//    {
//        private DrawingContext dc;
//        private Stack<State> stack;        
//        private List<Point> surfacePoints;        

//        private class State
//        {
//            private Pen pen;

//            public Matrix3D Rotation { get; set; }
//            public Point Position { get; set; }
//            public Pen Pen { get { return this.pen; } }
            
//            public double Thickness 
//            {
//                get { return this.pen.Thickness; }
//                set
//                {
//                    if (this.Thickness != value)
//                    {
//                        this.pen = new Pen(new SolidColorBrush(this.Color), value);
//                    }
//                }
//            }
            
//            public Color Color
//            {    
//                get
//                {
//                    return (this.pen.Brush as SolidColorBrush).Color;
//                }
//                set
//                {   
//                    if (this.Color != value)
//                    {
//                        this.pen = new Pen(new SolidColorBrush(value), this.Thickness);
//                    }                    
//                }
//            }
            
//            public State()
//            {
//                this.Position = new Point(0, 0);
//                this.Rotation = Matrix3D.Identity;
//                this.pen = new Pen(new SolidColorBrush(Colors.Black), 1);
//            }

//            public State(State other)
//            {
//                this.Position = new Point(other.Position.X, other.Position.Y);
//                this.Rotation = other.Rotation * Matrix3D.Identity;
//                this.pen = other.pen.Clone();
//            }
//        }

//        private Turtle2d(DrawingContext dc)
//        {
//            this.dc = dc;
//            this.stack = new Stack<State>();            

//            this.Push();
//        }

//        public Rect Bounds
//        {
//            get;
//            protected set;
//        }

//        private Pen Pen
//        {
//            get { return stack.Peek().Pen; }
//        }

//        private Point Position
//        {
//            get { return stack.Peek().Position; }
//            set
//            {
//                stack.Peek().Position = value;

//                Rect b = this.Bounds;
//                b.Union(value);
//                this.Bounds = b;
//            }
//        }

//        private Matrix3D Rotation
//        {
//            get { return stack.Peek().Rotation; }
//            set { stack.Peek().Rotation = value; }
//        }

//        public void Push()
//        {
//            if (stack.Count > 0)
//            {
//                stack.Push(new State(stack.Peek()));
//            }
//            else
//            {
//                stack.Push(new State());
//            }
//        }

//        public void Pop()
//        {
//            stack.Pop();
//        }

//        public void Forward(double distance, bool drawLine)
//        {
//            var offset = Rotation.Transform(new Point3D(distance, 0, 0));

//            var newPosition = this.Position;
//            newPosition.Offset(offset.X, offset.Y);

//            if (drawLine)
//            {
//                if (this.dc != null)
//                {
//                    // TODO: use drawing instead of calling DrawLine every time.
//                    this.dc.DrawLine(this.Pen, this.Position, newPosition);
//                }
//            }

//            if (this.surfacePoints != null)
//            {
//                this.surfacePoints.Add(newPosition);
//            }

//            this.Position = newPosition;
//        }

//        public void Move(double x, double y, double z)
//        {
//            this.Position = new Point(x, y);
//        }

//        public void MoveRel(double x, double y, double z)
//        {
//            Point newPosition = this.Position;

//            newPosition.Offset(x, y);

//            this.Position = newPosition;
//        }

//        public void Turn(double angle)
//        {
//            this.Rotation = Matricies.TurnMartix(angle) * this.Rotation;
//        }

//        public void Pitch(double angle)
//        {
//            this.Rotation = Matricies.PitchMartix(angle) * this.Rotation;
//        }

//        public void Roll(double angle)
//        {
//            this.Rotation = Matricies.RollMartix(angle) * this.Rotation;
//        }

//        public void SetThickness(double thickness)
//        {
//            this.stack.Peek().Thickness = thickness;
//        }

//        public double GetThickness()
//        {
//            return this.stack.Peek().Thickness;
//        }


//        public void SetColor(double r, double g, double b)
//        {
//            this.stack.Peek().Color = Color.FromRgb(
//                (byte)(255 * r),
//                (byte)(255 * g),
//                (byte)(255 * b));
//        }

//        public void SetThicknessAndColor(double thickness, double r, double g, double b)
//        {
//            this.stack.Peek().Thickness = thickness;
//            this.stack.Peek().Color = Color.FromRgb(
//                (byte)(255 * r),
//                (byte)(255 * g),
//                (byte)(255 * b));            
//        }

//        public void SurfaceBegin()
//        {
//            this.surfacePoints = new List<Point>();
//            this.surfacePoints.Add(this.Position);
//        }

//        public void SurfaceEnd()
//        {
//            if (this.surfacePoints != null && this.surfacePoints.Count > 0)
//            {
//                if (this.dc != null)
//                {
//                    PathFigure figure = new PathFigure();
//                    figure.StartPoint = this.surfacePoints[0];

//                    PathSegmentCollection segments = new PathSegmentCollection();
//                    for (int i = 1; i < this.surfacePoints.Count; i++)
//                        segments.Add(new LineSegment(this.surfacePoints[i], true));

//                    figure.Segments = segments;
                    
//                    PathGeometry geometry = new PathGeometry();
//                    geometry.Figures.Add(figure);
                
//                    this.dc.DrawGeometry(this.Pen.Brush, this.Pen, geometry);
//                }
//            }

//            this.surfacePoints = null;
//        }

//        private class Pair<T, U>
//        {
//            public Pair()
//            {
//            }

//            public Pair(T first, U second)
//            {
//                this.First = first;
//                this.Second = second;
//            }

//            public T First { get; set; }
//            public U Second { get; set; }
//        }

//        public static void Render(object system, DrawingContext dc, out Rect boundingRect)
//        {
//            var turtle = new Turtle2d(dc);

//            LSystems.System lSystem = (LSystems.System)system;

//            if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
//            {
//                ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = turtle;
//            }

//            var interpreter = new LSystems.Turtle.Interpreter();

//            interpreter.RegisterExternalFunctions(lSystem.Definition);

//            interpreter.Interpret(turtle, lSystem.String as IEnumerable);

//            if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
//            {
//                ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = null;
//            }

//            boundingRect = turtle.Bounds;
//        }
//    }
//}
