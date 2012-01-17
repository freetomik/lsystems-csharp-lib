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

        private List<OpenTK.Vector3> helperLines = new List<OpenTK.Vector3>();

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

        private const int MinEdgeCount = 3;
        private const int DefaultEdgeCount = 12;
        private const int MaxEdgeCount = 16;

        void OpenGLViewContainerOnLoad(object sender, EventArgs e)
        {
            UpdateProjectionButtons();
            UpdateModeButtons();

            for (int i = MinEdgeCount; i <= MaxEdgeCount; ++i)
            {
                this.toolStripComboBoxNumEdges.Items.Add(i.ToString());
            }

            this.toolStripComboBoxNumEdges.SelectedIndex = (DefaultEdgeCount - MinEdgeCount);
            

            this.loaded = true;
            
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Light0);

            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.0f, .0f, .0f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0, 0, 0, 1 });

            // Position the light.            
            GL.Light(LightName.Light0, LightParameter.Position, new float[] { 0, 0, 1, 0 });

            var materialKa = new OpenTK.Vector4(1, 1, 1, 1);
            var materialKd = new OpenTK.Vector4(1, 1, 1, 1);
            var materialKe = new OpenTK.Vector4(0, 0, 0, 1);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, materialKa);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, materialKd);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, materialKe);
            
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Lighting);            

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

                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Orange);
                for (int i = 0; i < helperLines.Count; i += 2)
                {
                    GL.Vertex3(helperLines[i]);
                    GL.Vertex3(helperLines[i + 1]);
                }
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

            this.helperLines.Clear();

            ResetState();

            this.Push();
            
            if (this.system != null)
            {
                //Debug.WriteLine("---------------------");
                //foreach (object obj in (IEnumerable)this.system.String)
                //{
                //    Debug.WriteLine(string.Format("{0}", obj.GetType()));
                //}                

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

            // Generate normals.
            for (int i = 0; i < vertexes.Count; i += 12)
            {
                OpenTK.Vector3 normal = CalcNormal(
                    new OpenTK.Vector3(vertexes[i], vertexes[i + 1], vertexes[i + 2]),
                    new OpenTK.Vector3(vertexes[i + 3], vertexes[i + 4], vertexes[i + 5]),
                    new OpenTK.Vector3(vertexes[i + 6], vertexes[i + 7], vertexes[i + 8]));

                for (int j = 0; j < 4; ++j) { Add(normals, normal); }
            }

            for (int i = 0; i < triVertexes.Count; i += 9)
            {
                OpenTK.Vector3 normal = CalcNormal(
                    new OpenTK.Vector3(triVertexes[i], triVertexes[i + 1], triVertexes[i + 2]),
                    new OpenTK.Vector3(triVertexes[i + 3], triVertexes[i + 4], triVertexes[i + 5]),
                    new OpenTK.Vector3(triVertexes[i + 6], triVertexes[i + 7], triVertexes[i + 8]));

                for (int j = 0; j < 3; ++j) { Add(triNormals, normal); }
            }

            // Copy arrays in non-managed memory.
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


        private int numEdges = 4;
        private bool simpleMode = false;

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
            
            public List<OpenTK.Vector3> fPoints = new List<OpenTK.Vector3>();
            public OpenTK.Matrix4 fMatrix;

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
                this.fPoints = other.fPoints;
                this.fMatrix = other.fMatrix;

                other.fPoints = new List<OpenTK.Vector3>();                    
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

        private void BuildWalls(OpenTK.Vector3[] wallPoints, OpenTK.Graphics.Color4 color)
        {
            for (int i = 0; i < numEdges; ++i)
            {
                int a = i;
                int b = i < numEdges - 1 ? i + 1 : 0;
                
                Add(this.vertexes, wallPoints[b]);
                Add(this.vertexes, wallPoints[a]);
                Add(this.vertexes, wallPoints[a + numEdges]);
                Add(this.vertexes, wallPoints[b + numEdges]);

                Add(this.colors, color);
                Add(this.colors, color);
                Add(this.colors, color);
                Add(this.colors, color);                
            }
        }

        private void BuildUpperCap(OpenTK.Vector3[] points, OpenTK.Graphics.Color4 color)
        {
            for (int j = 2; j < numEdges; ++j)
            {
                Add(this.triVertexes, points[numEdges]);
                Add(this.triVertexes, points[numEdges + j]);
                Add(this.triVertexes, points[numEdges + j - 1]);

                Add(this.triColors, color);
                Add(this.triColors, color);
                Add(this.triColors, color);
            }
        }

        private void BuildLowerCap(OpenTK.Vector3[] points, OpenTK.Graphics.Color4 color)
        {
            for (int j = 2; j < numEdges; ++j)
            {
                Add(this.triVertexes, points[0]);
                Add(this.triVertexes, points[j - 1]);
                Add(this.triVertexes, points[j]);

                Add(this.triColors, color);
                Add(this.triColors, color);
                Add(this.triColors, color);
            }
        }       

        public void Pop()
        {
            // Finalize cap of the current branch.
            if (stack.Peek().fPoints.Count > 0)
            {
                BuildWalls(
                    stack.Peek().fPoints.ToArray(), 
                    stack.Peek().Color);

                BuildUpperCap(
                    stack.Peek().fPoints.ToArray(),
                    stack.Peek().Color);
            }

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

        OpenTK.Vector3 CalcNormal(OpenTK.Vector3 _v1, OpenTK.Vector3 _v2, OpenTK.Vector3 _v3)
        {
            OpenTK.Vector3 v1 = _v2 - _v1;
            OpenTK.Vector3 v2 = _v3 - _v1;

            OpenTK.Vector3 normal = OpenTK.Vector3.Cross(v1, v2);
            normal.Normalize();
            return normal;
        }

        float AngleBetween(OpenTK.Matrix4 m1, OpenTK.Matrix4 m2)
        {
            var v1 = OpenTK.Vector3.Transform(new OpenTK.Vector3(1, 0, 0), m1);
            var v2 = OpenTK.Vector3.Transform(new OpenTK.Vector3(1, 0, 0), m2);
            return OpenTK.Vector3.CalculateAngle(v1, v2);
        }

        OpenTK.Vector3 VectorProjection(OpenTK.Vector3 basis, OpenTK.Vector3 v)
        {
            float length = v.Length;

            OpenTK.Vector3 result = basis;
            result.Normalize();

            return result * ((float)OpenTK.Vector3.Dot(result, v));
        }

        private const float minAngle = 0.005f;

        public void Forward(double? distPtr, bool drawLine)
        {
            double distance = distPtr.HasValue ? distPtr.Value : this.Distance;
            if (drawLine)
            {
                float thickness = (float)(this.Thickness / 2);
                float length = (float)distance;

                var currentPosition = OpenTK.Vector3.Transform(zeroVector, stack.Peek().Translation);                

                OpenTK.Vector3[] points = new OpenTK.Vector3[numEdges * 2];

                if (this.stack.Peek().fPoints.Count > 0)
                {
                    OpenTK.Vector3 x1 = new OpenTK.Vector3(1, 0, 0);
                    OpenTK.Vector3 vx1 = OpenTK.Vector3.Transform(x1, this.stack.Peek().fMatrix);
                    OpenTK.Vector3 vx2 = OpenTK.Vector3.Transform(x1, this.stack.Peek().Rotation);

                    float angle = OpenTK.Vector3.CalculateAngle(vx1, vx2);
                    OpenTK.Vector3 rotateAxis = OpenTK.Vector3.Cross(vx1, vx2);
                
                    for (int i = 0; i < numEdges; ++i)
                    {
                        var p = this.stack.Peek().fPoints[numEdges + i] - currentPosition;
                        p.Normalize();

                        if (angle > minAngle)
                        {
                            p = OpenTK.Vector3.Transform(p, OpenTK.Matrix4.CreateFromAxisAngle(rotateAxis, angle));
                        }

                        p = p * thickness;
                        points[i] = currentPosition + p;
                        points[i + numEdges] = currentPosition + p + vx2 * length;
                    }           
                }
                else
                {
                    // Create corner points.                                
                    for (int i = 0; i < numEdges; ++i)
                    {
                        float angle = (float)((i + 0.5f) * (Math.PI * 2) / numEdges);
                        points[i].Y = (float)(thickness * Math.Sin(angle));
                        points[i].Z = (float)(thickness * Math.Cos(angle));
                        points[i].X = 0;

                        points[i] = currentPosition +
                            OpenTK.Vector3.Transform(points[i], stack.Peek().Rotation);

                        points[i + numEdges].Y = (float)(thickness * Math.Sin(angle));
                        points[i + numEdges].Z = (float)(thickness * Math.Cos(angle));
                        points[i + numEdges].X = length;

                        points[i + numEdges] = currentPosition +
                            OpenTK.Vector3.Transform(points[i + numEdges], stack.Peek().Rotation);
                    }
                }

                if (simpleMode)
                {
                    BuildUpperCap(points, stack.Peek().Color);
                    BuildLowerCap(points, stack.Peek().Color);
                }
                else if (this.stack.Peek().fPoints.Count == 0)
                {
                    BuildLowerCap(points, stack.Peek().Color);
                }

                OpenTK.Vector3[] wallPoints = null;

                if (simpleMode)
                {
                    wallPoints = points;
                }
                else
                {
                    var previousPoints = this.stack.Peek().fPoints;
                    if (previousPoints.Count > 0)
                    {
                        // Update end points of previous tube,
                        // and start points of this tube.
                        float angle = AngleBetween(this.stack.Peek().fMatrix, stack.Peek().Rotation);

                        if (angle > minAngle)
                        {
                            float r = angle / (float)(Math.PI / 2);
                            float scale = 1.0f + r * r * r;

                            OpenTK.Vector3 x1 = new OpenTK.Vector3(1, 0, 0);

                            OpenTK.Vector3 v1 = OpenTK.Vector3.Transform(x1, this.stack.Peek().fMatrix);
                            OpenTK.Vector3 v2 = OpenTK.Vector3.Transform(x1, this.stack.Peek().Rotation);
                            OpenTK.Vector3 v3 = (v1 + v2) * 0.5f;
                            v3.Normalize();

                            OpenTK.Vector3 noScaleDirection = OpenTK.Vector3.Cross(v1, v2);
                            OpenTK.Vector3 upDirection = OpenTK.Vector3.Cross(noScaleDirection, v3);

                            for (int i = 0; i < numEdges; ++i)
                            {
                                OpenTK.Vector3 p = (previousPoints[numEdges + i] + points[i]) * 0.5f;
                                OpenTK.Vector3 dir = (p - currentPosition);                              
                                p = currentPosition +
                                    VectorProjection(noScaleDirection, dir) +
                                    VectorProjection(upDirection, dir) * scale;

                                previousPoints[numEdges + i] = points[i] = p;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < numEdges; ++i)
                            {
                                OpenTK.Vector3 p = (previousPoints[numEdges + i] + points[i]) * 0.5f;
                                previousPoints[numEdges + i] = points[i] = p;
                            }
                        }

                        // Render previous tube.
                        wallPoints = previousPoints.ToArray();
                    }

                    this.stack.Peek().fPoints = points.ToList();
                    this.stack.Peek().fMatrix = stack.Peek().Rotation;
                }

                if (wallPoints != null)
                {
                    BuildWalls(wallPoints, stack.Peek().Color);                     
                }                
            }
            else
            {
                if (this.stack.Peek().fPoints.Count > 0)
                {
                    BuildWalls(
                       stack.Peek().fPoints.ToArray(),
                       stack.Peek().Color);

                    BuildUpperCap(
                        stack.Peek().fPoints.ToArray(),
                        stack.Peek().Color);

                    stack.Peek().fPoints.Clear();
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
                    Add(this.triVertexes, this.surfacePoints[0]);
                    Add(this.triVertexes, this.surfacePoints[1]);
                    Add(this.triVertexes, this.surfacePoints[2]);

                    Add(this.triColors, this.surfaceColors[0]);
                    Add(this.triColors, this.surfaceColors[1]);
                    Add(this.triColors, this.surfaceColors[2]);

                    Add(this.triVertexes, this.surfacePoints[2]);
                    Add(this.triVertexes, this.surfacePoints[1]);
                    Add(this.triVertexes, this.surfacePoints[0]);
              
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

        private void UpdateModeButtons()
        {
            this.simpleToolStripMenuItemRenderTypeSimple.Checked = this.simpleMode;
            this.simpleToolStripMenuItemRenderTypeSmooth.Checked = !this.simpleMode;

            this.toolStripDropDownButtonBuildType.Text = this.simpleMode
                ? "Simple"
                : "Smooth";
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

        private void simpleToolStripMenuItemRenderTypeSimple_Click(object sender, EventArgs e)
        {
            this.simpleMode = true;

            UpdateModeButtons();

            SetSystem(this.system);
        }

        private void simpleToolStripMenuItemRenderTypeSmooth_Click(object sender, EventArgs e)
        {
            this.simpleMode = false;

            UpdateModeButtons();

            SetSystem(this.system);
        }

        private void toolStripComboBoxNumEdges_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.numEdges = this.toolStripComboBoxNumEdges.SelectedIndex + MinEdgeCount;

            SetSystem(this.system);
        }
    }
}
