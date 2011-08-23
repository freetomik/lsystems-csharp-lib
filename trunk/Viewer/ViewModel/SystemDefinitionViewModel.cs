using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Viewer.ViewModel
{
    public class Step
    {
        public LSystems.System System { get; set; }
        public string Name { get; set; }
    }

    public class SystemDefinitionViewModel : BaseViewModel
    {
        private Type definitionType;
        private LSystems.System system;
        private ObservableCollection<Step> steps = new ObservableCollection<Step>();
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

        public object Steps
        {
            get
            {
                return steps;
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

                        this.steps.Insert(0, new Step() { System = system, Name =  string.Format("Step {0}", i + 1)});

                        system = system.Clone();
                    }
                }            
            }

            if (this.steps.Count > 0)
            {
                CollectionViewSource.GetDefaultView(this.steps).MoveCurrentTo(this.steps[0]);
            }            
            NotifyPropertyChanged("System");
        }
    }
}
