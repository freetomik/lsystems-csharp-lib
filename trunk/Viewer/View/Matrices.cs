using System.Windows.Media.Media3D;
using System;

namespace Viewer.View
{
    public class Matricies
    {
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static Matrix3D TurnMartix(double angle)
        {
            double rad = DegreeToRadian(angle);
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            return new Matrix3D(
                cos, sin, 0, 0,
                -sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static Matrix3D PitchMartix(double angle)
        {
            double rad = DegreeToRadian(angle);
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            return new Matrix3D(
                cos, 0, -sin, 0,
                0, 1, 0, 0,
                sin, 0, cos, 0,
                0, 0, 0, 1);
        }

        public static Matrix3D RollMartix(double angle)
        {
            double rad = DegreeToRadian(angle);
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            return new Matrix3D(
                1, 0, 0, 0,
                0, cos, -sin, 0,
                0, sin, cos, 0,
                0, 0, 0, 1);
        }
    }
}