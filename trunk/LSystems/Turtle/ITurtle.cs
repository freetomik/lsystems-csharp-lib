using System;

namespace LSystems.Turtle
{
    public interface ITurtle
    {
        void Push();
        void Pop();
        void Forward(double distance, bool drawLine);
        void Move(double x, double y);
        void MoveRel(double x, double y);
        void Turn(double u);
        void LineThickness(double thickness);
        void LineColor(double r, double g, double b);
        void LineParameters(double thickness, double r, double g, double b);
    }
}
