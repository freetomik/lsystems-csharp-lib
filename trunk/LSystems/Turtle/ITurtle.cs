using System;

namespace LSystems.Turtle
{
    public interface ITurtle
    {
        double Distance { get; set; }
        double Angle { get; set; }
        double Thickness { get; set; }
     
        void Push();
        void Pop();
        
        void Forward(double? distance, bool drawLine);

        void Move(double x, double y, double z);
        void MoveRel(double x, double y, double z);
        
        void Turn(double? angle, bool left);
        void Pitch(double? angle, bool up);
        void Roll(double? angle, bool clockwise);

        void SurfaceBegin();
        void SurfaceEnd();

        void SetColor(double r, double g, double b);        
    }
}
