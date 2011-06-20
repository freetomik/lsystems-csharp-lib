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

        public Interpreter()
        {            
            Register<StartBranchModule>(p => Turtle.Push());
            Register<EndBranchModule>(p => Turtle.Pop());
            Register<F>(p => Turtle.Forward(p.Value, true));
            Register<f>(p => Turtle.Forward(p.Value, false));
            Register<L>(p => Turtle.Turn(p.Value));
            Register<R>(p => Turtle.Turn(-p.Value));
            Register<PitchUp>(p => Turtle.Pitch(p.Value));
            Register<PitchDown>(p => Turtle.Pitch(-p.Value));
            Register<RollLeft>(p => Turtle.Roll(p.Value));
            Register<RollRight>(p => Turtle.Roll(-p.Value));
            Register<MoveTo>(p => Turtle.Move(p.Value.X, p.Value.Y, 0));
            Register<MoveToRel>(p => Turtle.MoveRel(p.Value.X, p.Value.Y, 0));
            Register<LineThickness>(p => Turtle.SetThickness(p.Value));
            Register<LineColor>(p => Turtle.SetColor(p.Value.R, p.Value.G, p.Value.B));
        }
        
        public void RegisterExternalFunctions(object interpreter)
        {
            foreach (var method in interpreter.GetType().GetMethods())
            {
                InterpretAttribute i =
                   (InterpretAttribute)Attribute.GetCustomAttribute(method, typeof(InterpretAttribute), false);

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
