using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystems.Turtle
{
    public class StringParser
    {
        public double Angle { get; set; }
        private double Distance { get; set; }

        public Func<char, object> CharToObject { get; set; }

        public StringParser()
        {
            this.CharToObject = (p => null);
            this.Angle = 90;
            this.Distance = 90;
        }

        public List<object> Produce(string text)
        {
            List<object> result = new List<object>();
            foreach (char c in text)
            {
                object item = TransformCharToObject(c);
                if (item != null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private object TransformCharToObject(char c)
        {
            return CharToObject(c) ?? DefaultReplacement(c);            
        }

        private object DefaultReplacement(char c)
        {
            switch (c)
            {
                default: return null;
                case '[': return new StartBranchModule();
                case ']': return new EndBranchModule();
                case '%': return new CutModule();
                case F.Letter: return new F(this.distance);
                case f.Letter: return new f(this.distance);
                case L.Letter: return new L(this.angle);
                case R.Letter: return new R(this.angle);
                case SurfaceBegin.Letter: return new SurfaceBegin();
                case SurfaceEnd.Letter: return new SurfaceEnd();                
            }
        }
    }
}
