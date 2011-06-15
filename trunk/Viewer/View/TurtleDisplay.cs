using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Viewer.View
{
    public class TurtleDisplay : FrameworkElement
    {
        private VisualCollection children;

        public object System
        {
            get { return GetValue(SystemProperty); }
            set { SetValue(SystemProperty, value); }
        }

        public static readonly DependencyProperty SystemProperty =
            DependencyProperty.Register(
            "System",
            typeof(object),
            typeof(TurtleDisplay),
            new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));

        public TurtleDisplay()
        {
            this.children = new VisualCollection(this);            
            this.children.Add(new DrawingVisual());
        }

        protected override int VisualChildrenCount
        {
            get { return this.children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= this.children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.children[index];
        }

        private static void OnDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as TurtleDisplay).UpdatePathGraphics();            
        }
        
        private void UpdatePathGraphics()
        {
            LSystems.System system = this.System as LSystems.System;
            
            Rect boundingRect = new Rect(0, 0, 0, 0);

            if (system != null && system.String != null)
            {
                using (var dc = (this.children[0] as DrawingVisual).RenderOpen())
                {
                    Turtle2d.Render(system, dc, out boundingRect);
                }
            }

            // Update own size and render transformation.
            this.Height = boundingRect.Height;
            this.Width = boundingRect.Width;

            this.RenderTransform = new TransformGroup()
            {
                Children = new TransformCollection()
                {
                    new TranslateTransform(-boundingRect.X, -boundingRect.Y),
                    new ScaleTransform(1, -1, 0, this.Height / 2)
                }
            };
            
            InvalidateVisual();
        }
    }
}
