using System;

namespace LSystems.Turtle
{
    public interface ITurtle
    {        
        void Push();
        void Pop();
        void Forward(double distance, bool drawLine);
        void Move(double x, double y, double z);
        void MoveRel(double x, double y, double z);
        void Turn(double angle);
        void Pitch(double angle);
        void Roll(double angle);

        void SetThickness(double thickness);
        void SetColor(double r, double g, double b);
        void SetThicknessAndColor(double thickness, double r, double g, double b);
    }
}
