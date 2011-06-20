using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections;

namespace Viewer.View
{
    public class TurtleDisplay3d : Viewport3D, LSystems.Turtle.ITurtle
    {
        public object System
        {
            get { return (object)GetValue(SystemProperty); }
            set { SetValue(SystemProperty, value); }
        }

        public static readonly DependencyProperty SystemProperty =
            DependencyProperty.Register(
            "System",
            typeof(object),
            typeof(TurtleDisplay3d),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        private static void OnDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as TurtleDisplay3d).UpdateGeometry();
        }

        public TurtleDisplay3d()
        {
            this.MouseDown += new System.Windows.Input.MouseButtonEventHandler(TurtleDisplay3d_MouseDown);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(TurtleDisplay3d_MouseMove);
            this.MouseUp += new MouseButtonEventHandler(TurtleDisplay3d_MouseUp);
            this.KeyDown += new KeyEventHandler(TurtleDisplay3d_KeyDown);
            this.Focusable = true;
        }

        void TurtleDisplay3d_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                Point3D pos = (this.Camera as ProjectionCamera).Position;
                pos.Offset(0, 0, -10);
                (this.Camera as ProjectionCamera).Position = pos;                   
            }
            else if (e.Key == Key.PageDown)
            {
                Point3D pos = (this.Camera as ProjectionCamera).Position;
                pos.Offset(0, 0, 10);
                (this.Camera as ProjectionCamera).Position = pos;                   
            }
        }

        private Point mousePos;        

        void TurtleDisplay3d_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!this.IsMouseCaptured)
                return;

            Point newPos = e.GetPosition(this);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                rotateOffset.Offset(newPos.X - mousePos.X, newPos.Y - mousePos.Y);
            }
            else
            {
                translateOffset.Offset(newPos.X - mousePos.X, newPos.Y - mousePos.Y);
            }
            
            this.mousePos = newPos;

            if (this.Children.Count < 2)
            {
                return;
            }

            Transform3DGroup transformGroup = new Transform3DGroup();
            
            transformGroup.Children.Add(
                new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(0, 1, 0), this.rotateOffset.X * 0.3)));

            transformGroup.Children.Add(
                new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(1, 0, 0), this.rotateOffset.Y * 0.3)));

            transformGroup.Children.Add(
                new TranslateTransform3D(translateOffset.X, -translateOffset.Y, 0));

            ModelVisual3D model = (ModelVisual3D)this.Children.Last();
            model.Transform = transformGroup;
        }

        void TurtleDisplay3d_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
            CaptureMouse();
            this.mousePos = e.GetPosition(this);
        }

        void TurtleDisplay3d_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
        }

        private void UpdateGeometry()
        {
            if (this.Children.Count > 1)
            {
                this.Children.Remove(this.Children.Last());
            }

            var group = new ModelVisual3D();
            this.Children.Add(group);

            group.Children.Add(
                CreateCube(CreateSurfaceMaterial(Colors.Red),
                new ScaleTransform3D(100, 1, 1),
                new TranslateTransform3D(50, 0, 0)));

            group.Children.Add(
                CreateCube(CreateSurfaceMaterial(Colors.Green),
                new ScaleTransform3D(1, 100, 1),
                new TranslateTransform3D(0, 50, 0)));

            group.Children.Add(
                CreateCube(CreateSurfaceMaterial(Colors.Blue),
                new ScaleTransform3D(1, 1, 100),
                new TranslateTransform3D(0, 0, 50)));

            this.Reset();

            LSystems.System lSystem = (LSystems.System)System;

            if (lSystem != null)
            {
                if (lSystem.Definition is LSystems.Turtle.SystemDefintion)
                {
                    ((LSystems.Turtle.SystemDefintion)lSystem.Definition).Turtle = this;
                }

                var interpreter = new LSystems.Turtle.Interpreter();

                interpreter.RegisterExternalFunctions(lSystem.Definition);

                interpreter.Interpret(this, lSystem.String as IEnumerable);

                if (lSystem.Definition is LSystems.Turtle.SystemDefintion)
                {
                    ((LSystems.Turtle.SystemDefintion)lSystem.Definition).Turtle = null;
                }
            }
        }

        private static Material CreateSurfaceMaterial(Color colour)
        {
            return new DiffuseMaterial(new SolidColorBrush(colour));
        }

        private static ModelVisual3D CreateCube(Material material, params Transform3D[] transformations)
        {
            var p0 = new Point3D(-0.5, -0.5, -0.5);
            var p1 = new Point3D(0.5, -0.5, -0.5);
            var p2 = new Point3D(0.5, -0.5, 0.5);
            var p3 = new Point3D(-0.5, -0.5, 0.5);
            var p4 = new Point3D(-0.5, 0.5, -0.5);
            var p5 = new Point3D(0.5, 0.5, -0.5);
            var p6 = new Point3D(0.5, 0.5, 0.5);
            var p7 = new Point3D(-0.5, 0.5, 0.5);

            var mesh = new MeshGeometry3D();

            //front side triangles
            CreateTriangleModel(mesh, p3, p2, p6);
            CreateTriangleModel(mesh, p3, p6, p7);
            //right side triangles
            CreateTriangleModel(mesh, p2, p1, p5);
            CreateTriangleModel(mesh, p2, p5, p6);
            //back side triangles
            CreateTriangleModel(mesh, p1, p0, p4);
            CreateTriangleModel(mesh, p1, p4, p5);
            //left side triangles
            CreateTriangleModel(mesh, p0, p3, p7);
            CreateTriangleModel(mesh, p0, p7, p4);
            //top side triangles
            CreateTriangleModel(mesh, p7, p6, p5);
            CreateTriangleModel(mesh, p7, p5, p4);
            //bottom side triangles
            CreateTriangleModel(mesh, p2, p3, p0);
            CreateTriangleModel(mesh, p2, p0, p1);

            var model = new ModelVisual3D();
            model.Content = new GeometryModel3D(mesh, material);

            if (transformations.Length > 0)
            {
                if (transformations.Length == 1)
                {
                    model.Transform = transformations[0];
                }
                else
                {
                    Transform3DGroup group = new Transform3DGroup();
                    transformations.ToList().ForEach(p => group.Children.Add(p));
                    model.Transform = group;
                }

                model.Transform.Freeze();
            }

            return model;
        }

        private static void CreateTriangleModel(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2)
        {
            int index = mesh.TriangleIndices.Count;

            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);

            mesh.TriangleIndices.Add(index + 0);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);
        }

        #region ITurtle Member

        private Stack<Transform3DGroup> stack;
        private Color color;
        private Material material;
        private double thickness;
        private Point rotateOffset;
        private Point translateOffset;

        private void Reset()
        {
            this.stack = new Stack<Transform3DGroup>();
            this.color = Colors.Black;
            this.material = CreateSurfaceMaterial(this.color);
            this.thickness = 1;
            this.rotateOffset = new Point(0, 0);
            this.translateOffset = new Point(0, 0);

            Push();
        }

        public void Push()
        {
            if (stack.Count > 0)
            {
                stack.Push(stack.Peek().Clone());
            }
            else
            {
                Transform3DGroup group = new Transform3DGroup();
                group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), -90)));
                group.Children.Add(new TranslateTransform3D());
                stack.Push(group);
            }
        }

        public void Pop()
        {
            stack.Pop();
        }

        public void Forward(double distance, bool drawLine)
        {
            var offset = stack.Peek().Children[0].Transform(new Point3D(0, distance / 2, 0));
            var currentPosition = stack.Peek().Children[1].Transform(new Point3D(0, 0, 0));

            if (drawLine)
            {
                (this.Children.Last() as ModelVisual3D).Children.Add(
                    CreateCube(
                        this.material
                        , new ScaleTransform3D(this.thickness, distance, this.thickness)
                        , stack.Peek().Children[0].Clone()
                        , new TranslateTransform3D(currentPosition.X + offset.X, currentPosition.Y + offset.Y, currentPosition.Z + offset.Z)
                        ));
            }

            stack.Peek().Children[1] = new MatrixTransform3D(
                stack.Peek().Children[1].Value *
                new TranslateTransform3D(offset.X * 2, offset.Y * 2, offset.Z * 2).Value);
        }

        public void Move(double x, double y, double z)
        {
            //throw new NotImplementedException();
        }

        public void MoveRel(double x, double y, double z)
        {
            //throw new NotImplementedException();
        }

        public void Turn(double angle)
        {
            stack.Peek().Children[0] = new MatrixTransform3D(
                new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle)).Value * stack.Peek().Children[0].Value);
        }

        public void Pitch(double angle)
        {
            stack.Peek().Children[0] = new MatrixTransform3D(
                new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(1, 0, 0), angle)).Value * stack.Peek().Children[0].Value);
        }

        public void Roll(double angle)
        {
            stack.Peek().Children[0] = new MatrixTransform3D(
                new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle)).Value * stack.Peek().Children[0].Value);
        }

        public void SetThickness(double thickness)
        {
            this.thickness = thickness;
        }

        public void SetColor(double r, double g, double b)
        {
            Color color = Color.FromRgb(
                (byte)(255 * r),
                (byte)(255 * g),
                (byte)(255 * b));

            if (color != this.color)
            {
                this.color = color;
                this.material = CreateSurfaceMaterial(color);
            }
        }

        public void SetThicknessAndColor(double thickness, double r, double g, double b)
        {
            SetThickness(thickness);
            SetColor(r, g, b);
        }

        #endregion
    }
}
