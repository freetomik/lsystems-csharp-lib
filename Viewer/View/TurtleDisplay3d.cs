//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Controls;
//using System.Windows;
//using System.Windows.Media.Media3D;
//using System.Windows.Media;
//using System.Windows.Input;
//using System.Collections;

//namespace Viewer.View
//{
//    public class TurtleDisplay3d : Viewport3D, LSystems.Turtle.ITurtle
//    {
//        public object System
//        {
//            get { return (object)GetValue(SystemProperty); }
//            set { SetValue(SystemProperty, value); }
//        }

//        public static readonly DependencyProperty SystemProperty =
//            DependencyProperty.Register(
//            "System",
//            typeof(object),
//            typeof(TurtleDisplay3d),
//            new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

//        private static void OnDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
//        {
//            (obj as TurtleDisplay3d).UpdateGeometry();
//        }

//        public UIElement MouseController
//        {
//            get { return (UIElement)GetValue(MouseControllerProperty); }
//            set { SetValue(MouseControllerProperty, value); }
//        }
        
//        public static readonly DependencyProperty MouseControllerProperty =
//            DependencyProperty.Register("MouseController", typeof(UIElement), typeof(TurtleDisplay3d),
//            new UIPropertyMetadata(null, new PropertyChangedCallback(OnMouseControllerChanged)));

//        private static void OnMouseControllerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
//        {
//            var display = (obj as TurtleDisplay3d);

//            display.MouseController.MouseDown += new System.Windows.Input.MouseButtonEventHandler(display.TurtleDisplay3d_MouseDown);
//            display.MouseController.MouseMove += new System.Windows.Input.MouseEventHandler(display.TurtleDisplay3d_MouseMove);
//            display.MouseController.MouseUp += new MouseButtonEventHandler(display.TurtleDisplay3d_MouseUp);
//            display.MouseController.KeyDown += new KeyEventHandler(display.TurtleDisplay3d_KeyDown);
//            display.MouseController.Focusable = true;
//        }

//        public bool ResetCameraAutomatically
//        {
//            get { return (bool)GetValue(ResetCameraAutomaticallyProperty); }
//            set { SetValue(ResetCameraAutomaticallyProperty, value); }
//        }
        
//        public static readonly DependencyProperty ResetCameraAutomaticallyProperty =
//            DependencyProperty.Register("ResetCameraAutomatically", typeof(bool), typeof(TurtleDisplay3d), new UIPropertyMetadata(true));

//        public ICommand ResetCameraCommand
//        {
//            get { return new ViewModel.RelayCommand(p => this.ResetCamera()); }
//        }


//        public TurtleDisplay3d()
//        {            
//        }

//        void TurtleDisplay3d_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.Key == Key.PageUp)
//            {
//                Point3D pos = (this.Camera as ProjectionCamera).Position;
//                pos.Offset(0, 0, -10);
//                (this.Camera as ProjectionCamera).Position = pos;                   
//            }
//            else if (e.Key == Key.PageDown)
//            {
//                Point3D pos = (this.Camera as ProjectionCamera).Position;
//                pos.Offset(0, 0, 10);
//                (this.Camera as ProjectionCamera).Position = pos;                   
//            }
//        }

//        private Point mousePos;        

//        void TurtleDisplay3d_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
//        {
//            if (null == this.MouseController)
//                return;

//            if (!this.MouseController.IsMouseCaptured)
//                return;

//            Point newPos = e.GetPosition(this);
//            if (e.LeftButton == MouseButtonState.Pressed)
//            {
//                rotateOffset.Offset(newPos.X - mousePos.X, newPos.Y - mousePos.Y);
//            }
//            else
//            {
//                translateOffset.Offset(newPos.X - mousePos.X, newPos.Y - mousePos.Y);
//            }
            
//            this.mousePos = newPos;

//            if (this.Children.Count < 2)
//            {
//                return;
//            }

//            UpdateTransform();            
//        }

//        private void UpdateTransform()
//        {
//            Transform3DGroup transformGroup = new Transform3DGroup();

//            transformGroup.Children.Add(
//                new RotateTransform3D(
//                    new AxisAngleRotation3D(new Vector3D(0, 1, 0), this.rotateOffset.X * 0.3)));

//            transformGroup.Children.Add(
//                new RotateTransform3D(
//                    new AxisAngleRotation3D(new Vector3D(1, 0, 0), this.rotateOffset.Y * 0.3)));

//            transformGroup.Children.Add(
//                new TranslateTransform3D(translateOffset.X, -translateOffset.Y, 0));

//            ModelVisual3D model = (ModelVisual3D)this.Children.Last();
//            model.Transform = transformGroup;
//        }

//        void TurtleDisplay3d_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
//        {
//            Keyboard.Focus(MouseController);
//            this.MouseController.CaptureMouse();
//            this.mousePos = e.GetPosition(this);
//        }

//        void TurtleDisplay3d_MouseUp(object sender, MouseButtonEventArgs e)
//        {
//            this.MouseController.ReleaseMouseCapture();
//        }

//        private void UpdateGeometry()
//        {
//            if (this.Children.Count > 1)
//            {
//                this.Children.Remove(this.Children.Last());
//            }

//            var group = new ModelVisual3D();
//            this.Children.Add(group);

//            group.Children.Add(
//                CreateCube(CreateSurfaceMaterial(Colors.Red),
//                new ScaleTransform3D(100, 1, 1),
//                new TranslateTransform3D(50, 0, 0)));

//            group.Children.Add(
//                CreateCube(CreateSurfaceMaterial(Colors.Green),
//                new ScaleTransform3D(1, 100, 1),
//                new TranslateTransform3D(0, 50, 0)));

//            group.Children.Add(
//                CreateCube(CreateSurfaceMaterial(Colors.Blue),
//                new ScaleTransform3D(1, 1, 100),
//                new TranslateTransform3D(0, 0, 50)));

//            this.Reset();

//            LSystems.System lSystem = (LSystems.System)System;

//            if (lSystem != null)
//            {
//                if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
//                {
//                    ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = this;
//                }

//                var interpreter = new LSystems.Turtle.Interpreter();

//                interpreter.RegisterExternalFunctions(lSystem.Definition);

//                interpreter.Interpret(this, lSystem.String as IEnumerable);        

//                if (lSystem.Definition is LSystems.Turtle.SystemDefinition)
//                {
//                    ((LSystems.Turtle.SystemDefinition)lSystem.Definition).Turtle = null;
//                }
//            }

//            if (ResetCameraAutomatically)
//            {
//                ResetCamera();
//            }                 
//            else
//            {
//                UpdateTransform();
//            }
//        }

//        private void ResetCamera()
//        {
//            this.rotateOffset = new Point(0, 0);
//            this.translateOffset = new Point(0, 0);

//            // Update camera position and offset, so that complete object will be visible.
//            this.translateOffset.Y = bounds.Bottom - bounds.Height / 2;
//            this.UpdateTransform();

//            Point3D pos = (this.Camera as ProjectionCamera).Position;
//            pos.Z = 1.8 * bounds.Height;
//            (this.Camera as ProjectionCamera).Position = pos; 
//        }

//        private static Material CreateSurfaceMaterial(Color colour)
//        {
//            return new DiffuseMaterial(new SolidColorBrush(colour));
//        }

//        private static ModelVisual3D CreateCube(Material material, params Transform3D[] transformations)
//        {
//            var p0 = new Point3D(-0.5, -0.5, -0.5);
//            var p1 = new Point3D(0.5, -0.5, -0.5);
//            var p2 = new Point3D(0.5, -0.5, 0.5);
//            var p3 = new Point3D(-0.5, -0.5, 0.5);
//            var p4 = new Point3D(-0.5, 0.5, -0.5);
//            var p5 = new Point3D(0.5, 0.5, -0.5);
//            var p6 = new Point3D(0.5, 0.5, 0.5);
//            var p7 = new Point3D(-0.5, 0.5, 0.5);

//            var mesh = new MeshGeometry3D();

//            //front side triangles
//            CreateTriangleModel(mesh, p3, p2, p6);
//            CreateTriangleModel(mesh, p3, p6, p7);
//            //right side triangles
//            CreateTriangleModel(mesh, p2, p1, p5);
//            CreateTriangleModel(mesh, p2, p5, p6);
//            //back side triangles
//            CreateTriangleModel(mesh, p1, p0, p4);
//            CreateTriangleModel(mesh, p1, p4, p5);
//            //left side triangles
//            CreateTriangleModel(mesh, p0, p3, p7);
//            CreateTriangleModel(mesh, p0, p7, p4);
//            //top side triangles
//            CreateTriangleModel(mesh, p7, p6, p5);
//            CreateTriangleModel(mesh, p7, p5, p4);
//            //bottom side triangles
//            CreateTriangleModel(mesh, p2, p3, p0);
//            CreateTriangleModel(mesh, p2, p0, p1);

//            var model = new ModelVisual3D();
//            model.Content = new GeometryModel3D(mesh, material);

//            if (transformations.Length > 0)
//            {
//                if (transformations.Length == 1)
//                {
//                    model.Transform = transformations[0];
//                }
//                else
//                {
//                    Transform3DGroup group = new Transform3DGroup();
//                    transformations.ToList().ForEach(p => group.Children.Add(p));
//                    model.Transform = group;
//                }

//                model.Transform.Freeze();
//            }

//            return model;
//        }

//        private ModelVisual3D CreateSurface()
//        {
//            var mesh = new MeshGeometry3D();

//            for (int i = 2; i < this.surfacePoints.Count; ++i)
//            {
//                CreateTriangleModel(mesh,
//                    this.surfacePoints[0],
//                    this.surfacePoints[i],
//                    this.surfacePoints[i - 1]);

//                CreateTriangleModel(mesh,
//                    this.surfacePoints[0],
//                    this.surfacePoints[i - 1],
//                    this.surfacePoints[i]);
//            }

//            var model = new ModelVisual3D();

//            model.Content = new GeometryModel3D(mesh, this.stack.Peek().Material);

//            return model;

//        }

//        private static void CreateTriangleModel(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2)
//        {
//            int index = mesh.TriangleIndices.Count;

//            mesh.Positions.Add(p0);
//            mesh.Positions.Add(p1);
//            mesh.Positions.Add(p2);

//            mesh.TriangleIndices.Add(index + 0);
//            mesh.TriangleIndices.Add(index + 1);
//            mesh.TriangleIndices.Add(index + 2);
//        }

//        #region ITurtle Member

//        private Stack<State> stack;        
        
//        private Rect bounds;
//        private Point rotateOffset;
//        private Point translateOffset;
//        private List<Point3D> surfacePoints;

//        private class State
//        {
//            private Transform3DGroup group;
//            private Color color;

//            public double Thickness { get; set; }
//            public Transform3D Rotation
//            {
//                get { return this.group.Children[0]; }
//                set { this.group.Children[0] = value; }
//            }
//            public Transform3D Translation
//            {
//                get { return this.group.Children[1]; }
//                set { this.group.Children[1] = value; }
//            }

//            public Material Material { get; private set; }
//            public Color Color
//            {
//                get { return this.color; }
//                set
//                {
//                    if (this.color != value)
//                    {
//                        this.color = value;
//                        UpdateMaterial();
//                    }
//                }
//            }

//            private void UpdateMaterial()
//            {
//                this.Material = TurtleDisplay3d.CreateSurfaceMaterial(this.color);
//            }

//            public State()
//            {
//                this.Thickness = 1;
//                this.color = Colors.Black;

//                UpdateMaterial();

//                this.group = new Transform3DGroup();
//                group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0)));
//                group.Children.Add(new TranslateTransform3D());
//            }

//            public State(State other)
//            {
//                this.group = other.group.Clone();
//                this.Thickness = other.Thickness;
//                this.color = other.color;
//                UpdateMaterial();
//            }
//        }

//        private void Reset()
//        {
//            this.stack = new Stack<State>();            
//            this.bounds = new Rect();

//            if (this.ResetCameraAutomatically)
//            {
//                this.rotateOffset = new Point(0, 0);
//                this.translateOffset = new Point(0, 0);
//            }            

//            Push();
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
//            var offset = stack.Peek().Rotation.Transform(new Point3D(distance / 2, 0, 0));
//            var currentPosition = stack.Peek().Translation.Transform(new Point3D(0, 0, 0));

//            if (drawLine)
//            {
//                double thickness = this.stack.Peek().Thickness;
//                (this.Children.Last() as ModelVisual3D).Children.Add(
//                    CreateCube(
//                        this.stack.Peek().Material
//                        , new ScaleTransform3D(distance, thickness, thickness)
//                        , stack.Peek().Rotation.Clone()
//                        , new TranslateTransform3D(currentPosition.X + offset.X, currentPosition.Y + offset.Y, currentPosition.Z + offset.Z)
//                        ));
//            }

//            stack.Peek().Translation = new MatrixTransform3D(
//                new TranslateTransform3D(offset.X * 2, offset.Y * 2, offset.Z * 2).Value *
//                stack.Peek().Translation.Value);

//            var currentPos = stack.Peek().Translation.Transform(new Point3D(0, 0, 0));
//            this.bounds.Union(new Point(currentPos.X, currentPos.Y));

//            if (this.surfacePoints != null)
//            {
//                this.surfacePoints.Add(currentPos);
//            }
//        }

//        public void Move(double x, double y, double z)
//        {
//            stack.Peek().Translation = new MatrixTransform3D(
//                new TranslateTransform3D(x, y, z).Value);
//        }

//        public void MoveRel(double x, double y, double z)
//        {
//            stack.Peek().Translation = new MatrixTransform3D(
//                new TranslateTransform3D(x, y, z).Value *
//                stack.Peek().Translation.Value);
//        }

//        public void Turn(double angle)
//        {
//            stack.Peek().Rotation =
//                new MatrixTransform3D(Matricies.TurnMartix(angle) * stack.Peek().Rotation.Value);
//        }

//        public void Pitch(double angle)
//        {
//            stack.Peek().Rotation =
//                new MatrixTransform3D(Matricies.PitchMartix(angle) * stack.Peek().Rotation.Value);
//        }

//        public void Roll(double angle)
//        {
//            stack.Peek().Rotation =
//                new MatrixTransform3D(Matricies.RollMartix(angle) * stack.Peek().Rotation.Value);
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
//            SetThickness(thickness);
//            SetColor(r, g, b);
//        }

//        public void SurfaceBegin()
//        {
//            this.surfacePoints = new List<Point3D>();
//            this.surfacePoints.Add(this.stack.Peek().Translation.Transform(new Point3D(0, 0, 0)));
//        }

//        public void SurfaceEnd()
//        {
//            if (this.surfacePoints != null)
//            {
//                if (this.surfacePoints.Count > 0)
//                {
//                    (this.Children.Last() as ModelVisual3D).Children.Add(CreateSurface());
//                }          
//            }
//            this.surfacePoints = null;
//        }

//        #endregion
//    }
//}
