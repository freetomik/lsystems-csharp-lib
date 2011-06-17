using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystems
{
    public enum RewriteDirection
    {
        LeftToRight,
        RightToLeft
    }

    public interface IRewriteRules
    {
        object Axiom { get; }
        int Depth { get; }
        RewriteDirection RewriteDirection { get; }
    }
}
