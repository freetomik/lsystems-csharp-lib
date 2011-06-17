using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.ViewModel
{
    public class SystemDefinitionViewModel : BaseViewModel
    {
        private Type definitionType;
        private LSystems.System system;
        private LSystems.SystemDefinition definition;
        
        public SystemDefinitionViewModel(Type type)
        {
            this.definitionType = type;
        }

        public string Name
        {
            get 
            {
                return definitionType.Name;
            }
        }

        public object System
        {
            get
            {
                return system;
            }
        }

        public void Build()
        {
            if (system == null)
            {
                definition = (LSystems.SystemDefinition)Activator.CreateInstance(definitionType);
                try
                {
                    system = LSystems.SystemBuilder.BuildSystem(definition);
                }
                catch (InvalidOperationException)
                {
                	// Failure.
                    return;
                }
            }
            else
            {
                return;
            }

            // Iterate.
            LSystems.IRewriteRules rules = definition as LSystems.IRewriteRules;
            if (rules != null)
            {
                system.String = rules.Axiom;

                if (system.String != null)
                {
                    for (int i = 0; i < rules.Depth; ++i)
                    {
                        switch (rules.RewriteDirection)
                        {
                            case LSystems.RewriteDirection.LeftToRight:
                                system.RewriteLeftToRight();
                                break;

                            case LSystems.RewriteDirection.RightToLeft:
                                system.RewriteRightToLeft();
                                break;

                            default:
                                throw new InvalidOperationException("Unknown RewriteDirection");
                        }
                        
                        system.Decomposite();
                    }
                }
            }            

            NotifyPropertyChanged("System");
        }
    }
}
