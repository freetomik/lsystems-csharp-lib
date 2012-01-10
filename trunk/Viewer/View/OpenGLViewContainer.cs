using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Windows;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Viewer.View
{
    public partial class OpenGLViewContainer : UserControl, LSystems.Turtle.ITurtle
    {
        private bool loaded = false;
        private LSystems.System system = null;

        private bool isOrtho = false;

        private IntPtr vertexesPtr = IntPtr.Zero;
        private IntPtr normalsPtr = IntPtr.Zero;
        private IntPtr colorsPtr = IntPtr.Zero;
        private List<float> vertexes = new List<float>();
        private List<float> normals = new List<float>();
        private List<float> colors = new List<float>();
        private int numQuadVertexes = 0;


        private IntPtr triVertexesPtr = IntPtr.Zero;
        private IntPtr triNormalsPtr = IntPtr.Zero;
        private IntPtr triColorsPtr = IntPtr.Zero;
        private List<float> triVertexes = new List<float>();
        private List<float> triNormals = new List<float>();
        private List<float> triColors = new List<float>();
        private int numTriVertexes = 0;

        private OpenTK.Vector3 minPoint;
        private OpenTK.Vector3 maxPoint;
                
        private System.Drawing.Point mousePos = new System.Drawing.Point(0, 0);
        private OpenTK.Vector3 cameraAngle = new OpenTK.Vector3(0, 0, 0);

        public OpenGLViewContainer()
        {
            InitializeComponent();

            this.Load += new EventHandler(OpenGLViewContainerOnLoad);

            this.glControl1.Paint += new PaintEventHandler(glControl1_Paint);
            this.glControl1.Resize += new EventHandler(glControl1_Resize);

            this.glControl1.MouseDown += new MouseEventHandler(glControl1_MouseDown);
            this.glControl1.MouseUp +=new MouseEventHandler(glControl1_MouseUp);
            this.glControl1.MouseMove += new MouseEventHandler(glControl1_MouseMove);
        }

        void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            this.glControl1.Capture = true;
            this.mousePos = e.Location;
        }

        void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            this.glControl1.Capture = false;
        }
        

        void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.glControl1.Capture)
            {
                var newPos = e.Location;

                this.cameraAngle.X += (newPos.Y - this.mousePos.Y) * 0.5f;
                this.cameraAngle.Y += (newPos.X - this.mousePos.X) * 0.5f;

                this.glControl1.Invalidate();

                this.mousePos = newPos;
            }
        }


        void OpenGLViewContainerOnLoad(object sender, EventArgs e)
        {
            UpdateProjectionButtons();

            this.loaded = true;
            
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Light0);

            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.0f, .0f, .0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0, 0, 0, 1 });

            // Position the light.
            //var lightPos = new OpenTK.Vector4(0, 0, -1, 0);
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0, 0, 1, 0 });

            var materialKa = new OpenTK.Vector4(1, 1, 1, 1);
            var materialKd = new OpenTK.Vector4(1, 1, 1, 1);
            var materialKe = new OpenTK.Vector4(0, 0, 0, 1);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, materialKa);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, materialKd);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, materialKe);
            
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            //GL.Enable(EnableCap.AutoNormal);
            GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);

            GL.Enable(EnableCap.Lighting);            

            //GL.ShadeModel(ShadingModel.Flat);
            //GL.LightModel(LightModelParameter.LightModelTwoSide, 1);

            float[] a = new float [] {0.3f, 0.3f, 0.3f, 1.0f };         
            GL.LightModel(LightModelParameter.LightModelAmbient, a);
        }

        void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
        }


        void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);                       

            if (this.system != null)
            {
                // Setup camera.
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();

                OpenTK.Vector3 bounds = this.maxPoint - this.minPoint;

                float controlWidth = this.glControl1.Width;
                float controlHeight = this.glControl1.Height;

                float controlAspect = controlWidth / controlHeight;
                
                float maxSize = bounds.Length * 0.5f;
                float orthoWidth = 0, orthoHeight = 0;
                if (controlWidth > controlHeight)
                {
                    orthoHeight = maxSize;
                    orthoWidth = maxSize * controlAspect;

                }
                else 
                {
                    orthoWidth = maxSize;
                    orthoHeight = maxSize / controlAspect;
                }
                
                if (isOrtho)
                {
                    GL.Ortho(-orthoWidth, orthoWidth, -orthoHeight, orthoHeight, -100000, 100000);
                }
                else
                {
                    OpenTK.Matrix4 m = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)(Math.PI * 0.3f), controlAspect, 1, 2 + orthoHeight * 10);
                    GL.MultMatrix(ref m);
                }

                // Render L-System.
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                if (!isOrtho)
                {
                    OpenTK.Matrix4 lookAt = OpenTK.Matrix4.LookAt(0, 0, orthoHeight * 2, 0, 0, 0, 0, 1, 0);
                    GL.MultMatrix(ref lookAt);
                }

                GL.Rotate(cameraAngle.X, 1, 0, 0);
                GL.Rotate(cameraAngle.Y, 0, 1, 0);

                GL.Translate((this.maxPoint + this.minPoint) * (-0.5f));

                // Draw axis.
                float axisLength = Math.Max(orthoWidth, orthoHeight);
                GL.Disable(EnableCap.Lighting);
                GL.Begin(BeginMode.Lines);
                
                GL.Color3(Color.Red);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(axisLength, 0, 0);

                GL.Color3(Color.Green);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, axisLength, 0);

                GL.Color3(Color.Blue);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, axisLength);
                GL.End();
                
                GL.Enable(EnableCap.Lighting);

                Render(
                    BeginMode.Quads,
                    this.numQuadVertexes,
                    this.vertexesPtr,
                    this.colorsPtr,
                    this.normalsPtr,
                    null,
                    null);
                
                Render(
                    BeginMode.Triangles,
                    this.numTriVertexes,
                    this.triVertexesPtr,
                    this.triColorsPtr,
                    this.triNormalsPtr,
                    null,
                    null);
            }

            glControl1.SwapBuffers();
        }

        void Render(BeginMode mode, int count, IntPtr vs, IntPtr cs, IntPtr ns, List<float> _vertexes, List<float> _normals)
        {
            if (vs != IntPtr.Zero) GL.EnableClientState(ArrayCap.VertexArray);
            if (ns != IntPtr.Zero) GL.EnableClientState(ArrayCap.NormalArray);
            if (cs != IntPtr.Zero) GL.EnableClientState(ArrayCap.ColorArray);

            GL.NormalPointer(NormalPointerType.Float, 0, ns);
            GL.ColorPointer(3, ColorPointerType.Float, 0, cs);
            GL.VertexPointer(3, VertexPointerType.Float, 0, vs);

            GL.DrawArrays(mode, 0, count);

            if (vs != IntPtr.Zero) GL.DisableClientState(ArrayCap.VertexArray);
            if (cs != IntPtr.Zero) GL.DisableClientState(ArrayCap.ColorArray);
            if (ns != IntPtr.Zero) GL.DisableClientState(ArrayCap.NormalArray);            

            // Test normals.
            if (_vertexes != null && _normals != null)
            {
                GL.Disable(EnableCap.Lighting);
                GL.Begin(BeginMode.Lines);
                for (int i = 0; i < _vertexes.Count; i += 3)
                {
                    GL.Vertex3(_vertexes[i], _vertexes[i + 1], _vertexes[i + 2]);

                    const float K = 100;

                    GL.Vertex3(
                        _vertexes[i] + _normals[i] * K,
                        _vertexes[i + 1] + _normals[i + 1] * K,
                        _vertexes[i + 2] + _normals[i + 2] * K);
                }
                GL.End();
                GL.Enable(EnableCap.Lighting);
            }                    
        }
        
        private void Clear(ref IntPtr memPtr)
        {
            if (memPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(memPtr);
                memPtr = IntPtr.Zero;
            }
        }

        private void Create(List<float> array, ref IntPtr refPtr)
        {
            refPtr = Marshal.AllocHGlobal(sizeof(float) * array.Count);
            Marshal.Copy(array.ToArray(), 0, refPtr, array.Count);
        }

        public void SetSystem(LSystems.System system)
        {
            this.system = system;

            // Rebuild system.
            Clear(ref this.vertexesPtr);
            Clear(ref this.colorsPtr);
            Clear(ref this.normalsPtr);
            Clear(ref this.triVertexesPtr);
            Clear(ref this.triColorsPtr);
            Clear(ref this.triNormalsPtr);

            this.numQuadVertexes = 0;
            this.numTriVertexes = 0;

            this.vertexes.Clear();
            this.normals.Clear();
            this.colors.Clear();
            this.triVertexes.Clear();
            this.triNormals.Clear();
            this.triColors.Clear();

            ResetState();

            this.Push();

            if (this.system != null)
            {
                if (this.system.Definition is LSystems.Turtle.SystemDefinition)
                {
                    ((LSystems.Turtle.SystemDefinition)this.system.Definition).Turtle = this;
                }

                var interpreter = new LSystems.Turtle.Interpreter();
                interpreter.RegisterExternalFunctions(this.system.Definition);
                interpreter.Interpret(this, this.system.String as IEnumerable);
                if (this.system.Definition is LSystems.Turtle.SystemDefinition)
                {
                    ((LSystems.Turtle.SystemDefinition)this.system.Definition).Turtle = null;
                }
            }

            this.Pop();

            Create(this.vertexes, ref this.vertexesPtr);
            Create(this.colors, ref this.colorsPtr);
            Create(this.normals, ref this.normalsPtr);
            
            this.numQuadVertexes = this.vertexes.Count / 3;

            Create(this.triVertexes, ref this.triVertexesPtr);
            Create(this.triColors, ref this.triColorsPtr);
            Create(this.triNormals, ref this.triNormalsPtr);

            this.numTriVertexes = this.triVertexes.Count / 3;

            this.toolStripLabelStatistics.Text =
                string.Format("{0} quads, {1} triangles.", this.numQuadVertexes / 4, this.numTriVertexes / 3);
            
            // Force redraw.
            glControl1.Invalidate();
        }

        #region ITurtle Member        

        private Stack<State> stack = new Stack<State>();
        private List<OpenTK.Vector3> surfacePoints;
        private List<OpenTK.Graphics.Color4> surfaceColors;


        private void ResetState()
        {
            this.stack.Clear();
            this.minPoint = zeroVector;
            this.maxPoint = zeroVector;
        }

        private class State
        {
            public OpenTK.Matrix4 Rotation { get; set; }
            public OpenTK.Matrix4 Translation { get; set; }
            public OpenTK.Graphics.Color4 Color { get; set; }
            public float Thickness { get; set; }
            public float Distance { get; set; }
            public float Angle { get; set; }
            
            public State()
            {
                this.Thickness = 1;
                this.Distance = 10;
                this.Angle = 90;
                this.Color = OpenTK.Graphics.Color4.White;
                this.Rotation = OpenTK.Matrix4.Identity;
                this.Translation = OpenTK.Matrix4.Identity;
            }

            public State(State other)
            {                
                this.Thickness = other.Thickness;
                this.Color = other.Color;
                this.Rotation = other.Rotation;
                this.Translation = other.Translation;
                this.Angle = other.Angle;
                this.Distance = other.Distance;
            }
        }

        public double Angle
        {
            get { return stack.Peek().Angle; }
            set { stack.Peek().Angle = (float)value; }
        }

        public double Distance
        {
            get { return stack.Peek().Distance; }
            set { stack.Peek().Distance = (float)value; }
        }

        public double Thickness
        {
            get { return stack.Peek().Thickness; }
            set { stack.Peek().Thickness = (float)value; }
        }
           
        public void Push()
        {
            if (stack.Count > 0)
            {
                stack.Push(new State(stack.Peek()));
            }
            else
            {
                stack.Push(new State());
            }
        }

        public void Pop()
        {
            stack.Pop();
        }

        private void Add(List<float> array, OpenTK.Vector3 v)
        {
            array.Add(v.X);
            array.Add(v.Y);
            array.Add(v.Z);
        }

        private void Add(List<float> array, OpenTK.Graphics.Color4 color)
        {
            array.Add(color.R);
            array.Add(color.G);
            array.Add(color.B);
        }

        static OpenTK.Vector3 zeroVector = new OpenTK.Vector3(0, 0, 0);

        private void Vertex(float x, float y, float z)
        {
            var currentPosition = OpenTK.Vector3.Transform(zeroVector, stack.Peek().Translation);
            var p = OpenTK.Vector3.Transform(new OpenTK.Vector3(x, y, z), stack.Peek().Rotation);

            Add(this.vertexes, p + currentPosition);
            Add(this.colors, stack.Peek().Color);            
        }

        static private int[] indexes = new int[] {
                    0, 3, 2, 1, // bottom
                    4, 5, 6, 7, // top
                    0, 1, 5, 4, 
                    1, 2, 6, 5, 
                    2, 3, 7, 6, 
                    3, 0, 4, 7
                };

        OpenTK.Vector3 CalcNormal(OpenTK.Vector3 _v1, OpenTK.Vector3 _v2, OpenTK.Vector3 _v3)
        {
            OpenTK.Vector3 v1 = _v2 - _v1;
            OpenTK.Vector3 v2 = _v3 - _v1;

            OpenTK.Vector3 normal = OpenTK.Vector3.Cross(v1, v2);
            normal.Normalize();
            return normal;
        }

        OpenTK.Vector3 CalcNormal(int surfaceIndex, float[] pts)
	    {
            int i1 = indexes[surfaceIndex * 4];
            int i2 = indexes[surfaceIndex * 4 + 1];
            int i3 = indexes[surfaceIndex * 4 + 2];            

            return CalcNormal(
                new OpenTK.Vector3(pts[i1 * 3], pts[i1 * 3 + 1], pts[i1 * 3 + 2]),
                new OpenTK.Vector3(pts[i2 * 3], pts[i2 * 3 + 1], pts[i2 * 3 + 2]),
                new OpenTK.Vector3(pts[i3 * 3], pts[i3 * 3 + 1], pts[i3 * 3 + 2]));
	    }

        public void Forward(double? distPtr, bool drawLine)
        {
            double distance = distPtr.HasValue ? distPtr.Value : this.Distance;
            if (drawLine)
            {
                float thickness = (float)(this.Thickness / 2);
                float length = (float)distance;
    
                float[] pts = new float[] {
                    0, thickness, thickness, 
                    0, -thickness, thickness, 
                    0, -thickness, -thickness, 
                    0, thickness, -thickness, 

                    length, thickness, thickness, 
                    length, -thickness, thickness, 
                    length, -thickness, -thickness, 
                    length, thickness, -thickness
                };

                for (int i = 0; i < indexes.Length; ++i)                
                {
                    int index = indexes[i] * 3;
                    Vertex(pts[index], pts[index + 1], pts[index + 2]);
                }

                for (int i = 0; i < 6; ++i)
                {
                    OpenTK.Vector3 normal = OpenTK.Vector3.Transform(CalcNormal(i, pts), stack.Peek().Rotation);
                    Add(this.normals, normal);
                    Add(this.normals, normal);
                    Add(this.normals, normal);
                    Add(this.normals, normal);                    
                }
            }

            var offset = OpenTK.Vector3.Transform(
                new OpenTK.Vector3((float)distance, 0, 0),
                stack.Peek().Rotation);

            stack.Peek().Translation = OpenTK.Matrix4.Mult(
                OpenTK.Matrix4.CreateTranslation(offset), 
                stack.Peek().Translation);
                
            var currentPos = OpenTK.Vector3.Transform(zeroVector, stack.Peek().Translation);
            this.minPoint = OpenTK.Vector3.ComponentMin(this.minPoint, currentPos);
            this.maxPoint = OpenTK.Vector3.ComponentMax(this.maxPoint, currentPos);

            if (this.surfacePoints != null)
            {
                this.surfacePoints.Add(currentPos);
                this.surfaceColors.Add(stack.Peek().Color);

                if (this.surfacePoints.Count == 3)
                {
                    // Build new triangle.
                    OpenTK.Vector3 normal = CalcNormal(
                        this.surfacePoints[0],
                        this.surfacePoints[1],
                        this.surfacePoints[2]);

                    Add(this.triVertexes, this.surfacePoints[0]);
                    Add(this.triVertexes, this.surfacePoints[1]);
                    Add(this.triVertexes, this.surfacePoints[2]);

                    Add(this.triNormals, normal);
                    Add(this.triNormals, normal);
                    Add(this.triNormals, normal);

                    Add(this.triColors, this.surfaceColors[0]);
                    Add(this.triColors, this.surfaceColors[1]);
                    Add(this.triColors, this.surfaceColors[2]);

                    Add(this.triVertexes, this.surfacePoints[2]);
                    Add(this.triVertexes, this.surfacePoints[1]);
                    Add(this.triVertexes, this.surfacePoints[0]);

                    normal = normal * (-1.0f);
                    Add(this.triNormals, normal);
                    Add(this.triNormals, normal);
                    Add(this.triNormals, normal);

                    Add(this.triColors, this.surfaceColors[2]);
                    Add(this.triColors, this.surfaceColors[1]);
                    Add(this.triColors, this.surfaceColors[0]);

                    this.surfacePoints.RemoveAt(1);
                    this.surfaceColors.RemoveAt(1);
                }
            }
        }

        public void Move(double x, double y, double z)
        {
            stack.Peek().Translation = OpenTK.Matrix4.CreateTranslation((float)x, (float)y, (float)z);
        }

        public void MoveRel(double x, double y, double z)
        {
            stack.Peek().Translation = OpenTK.Matrix4.Mult(
                OpenTK.Matrix4.CreateTranslation((float)x, (float)y, (float)z),
                stack.Peek().Translation);
        }

        public void Turn(double? anglePtr, bool left)
        {
            double angle = anglePtr.HasValue ? anglePtr.Value : this.Angle;
            if (!left) { angle = -angle; }
            stack.Peek().Rotation = OpenTK.Matrix4.Mult( 
                TurnMartix(angle),
                stack.Peek().Rotation);
        }

        public void Pitch(double? anglePtr, bool up)
        {
            double angle = anglePtr.HasValue ? anglePtr.Value : this.Angle;
            if (!up) { angle = -angle; }
            stack.Peek().Rotation = OpenTK.Matrix4.Mult( 
                PitchMartix(angle),
                stack.Peek().Rotation);
        }

        public void Roll(double? anglePtr, bool clockwise)
        {
            double angle = anglePtr.HasValue ? anglePtr.Value : this.Angle;
            if (!clockwise) { angle = -angle; }
            stack.Peek().Rotation = OpenTK.Matrix4.Mult(
                RollMartix(angle),
                stack.Peek().Rotation);
        }        

        public void SurfaceBegin()
        {
            this.surfacePoints = new List<OpenTK.Vector3>();
            this.surfaceColors = new List<OpenTK.Graphics.Color4>();

            this.surfacePoints.Add(OpenTK.Vector3.Transform(zeroVector, this.stack.Peek().Translation));
            this.surfaceColors.Add(this.stack.Peek().Color);
        }

        public void SurfaceEnd()
        {
            this.surfacePoints = null;
            this.surfaceColors = null;
        }

        public void SetColor(double r, double g, double b)
        {
            stack.Peek().Color = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }        

        #endregion


        private static float DegreeToRadian(double angle)
        {
            return (float)((Math.PI / 180.0f) * angle);
        }

        public static OpenTK.Matrix4 TurnMartix(double angle)
        {
            float rad = DegreeToRadian(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            return new OpenTK.Matrix4(
                cos, sin, 0, 0,
                -sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static OpenTK.Matrix4 PitchMartix(double angle)
        {
            float rad = DegreeToRadian(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            return new OpenTK.Matrix4(
                cos, 0, -sin, 0,
                0, 1, 0, 0,
                sin, 0, cos, 0,
                0, 0, 0, 1);
        }

        public static OpenTK.Matrix4 RollMartix(double angle)
        {
            float rad = DegreeToRadian(angle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            return new OpenTK.Matrix4(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1);
        }

        private void UpdateProjectionButtons()
        {
            this.toolStripButtonOrtho.Checked = isOrtho;
            this.toolStripButtonPerspective.Checked = !isOrtho;
        }

        private void toolStripButtonOrtho_Click(object sender, EventArgs e)
        {
            this.isOrtho = true;

            UpdateProjectionButtons();

            this.glControl1.Invalidate();
        }

        private void toolStripButtonPerspective_Click(object sender, EventArgs e)
        {
            this.isOrtho = false;

            UpdateProjectionButtons();

            this.glControl1.Invalidate();
        }

        private void toolStripButtonUndoCamera_Click(object sender, EventArgs e)
        {
            this.cameraAngle = new OpenTK.Vector3(0, 0, 0);

            this.glControl1.Invalidate();
        }
    }
}
