using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;

namespace LSystems.Turtle
{
    public class Interpreter
    {
        private Dictionary<Type, Action<object>> registeredTypes = new Dictionary<Type,Action<object>>();

        public ITurtle Turtle { get; set; }
        
        public void Register<T>(Action<T> action)
        {
            registeredTypes[typeof(T)] = p => action((T)p);
        }        

        static double? MValue(Module<double> m)
        {
            if (m.Initialized) return m.Value;
            else return null;
        }

        public Interpreter()
        {            
            Register<StartBranchModule>(p => Turtle.Push());
            Register<EndBranchModule>(p => Turtle.Pop());
            Register<F>(p => Turtle.Forward(MValue(p), true));
            Register<f>(p => Turtle.Forward(MValue(p), false));
            Register<TurnLeft>(p => Turtle.Turn(MValue(p), true));
            Register<TurnRight>(p => Turtle.Turn(MValue(p), false));
            Register<TurnAround>(p => Turtle.Turn(180, true));
            Register<PitchUp>(p => Turtle.Pitch(MValue(p), true));
            Register<PitchDown>(p => Turtle.Pitch(MValue(p), false));
            Register<RollLeft>(p => Turtle.Roll(MValue(p), true));
            Register<RollRight>(p => Turtle.Roll(MValue(p), false));
            Register<MoveTo>(p => Turtle.Move(p.Value.X, p.Value.Y, 0));
            Register<MoveToRel>(p => Turtle.MoveRel(p.Value.X, p.Value.Y, 0));
            Register<LineThickness>(p => Turtle.Thickness = p.Value);
            Register<LineColor>(p => Turtle.SetColor(p.Value.R, p.Value.G, p.Value.B));
            Register<SurfaceBegin>(p => Turtle.SurfaceBegin());
            Register<SurfaceEnd>(p => Turtle.SurfaceEnd());
            Register<Angle>(p => Turtle.Angle = p.Value);
            Register<Distance>(p => Turtle.Distance = p.Value);
        }
        
        public void RegisterExternalFunctions(object interpreter)
        {
            foreach (var method in interpreter.GetType().GetMethods())
            {
                InterpretAttribute i = (InterpretAttribute)
                    Attribute.GetCustomAttribute(method, typeof(InterpretAttribute), false);

                if (i == null)
                {
                    continue;
                }

                if (1 != method.GetParameters().Count())
                {
                    throw new InvalidOperationException("Interpret rule must have 1 parameter.");
                }

                if (method.ReturnType != typeof(void))
                {
                    throw new InvalidOperationException("Interpret rule must return void.");
                }

                Type paramType = method.GetParameters()[0].ParameterType;
                MethodInfo mi = method;
                Action<object> action = 
                    p => mi.Invoke(interpreter, new object [] { p });

                this.registeredTypes[paramType] = action;
            }
        }
        
        public void Interpret(ITurtle turtle, IEnumerable modules)
        {
            this.Turtle = turtle;
            foreach (var module in modules)
            {
                Action<object> action = null;
                if (this.registeredTypes.TryGetValue(module.GetType(), out action))
                {
                    action(module);
                }
            }
            this.Turtle = null;
        }
    }
}
