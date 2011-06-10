using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

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
            Register<MoveTo>(p => Turtle.Move(p.Value.X, p.Value.Y));
            Register<MoveToRel>(p => Turtle.MoveRel(p.Value.X, p.Value.Y));
            Register<LineThickness>(p => Turtle.LineThickness(p.Value));
            Register<LineColor>(p => Turtle.LineColor(p.Value.R, p.Value.G, p.Value.B));
        }

        public void Interpret(ITurtle turtle, IEnumerable modules)
        {
            foreach (var module in modules)
            {
                Action<object> action;
                if (registeredTypes.TryGetValue(module.GetType(), out action))
                {
                    action(module);
                }
            }
        }
    }
}
