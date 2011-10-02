using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

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
        private string systemError;
        
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

        public string SystemError
        {
            get
            {
                return systemError;
            }
            set
            {
                systemError = value;
                NotifyPropertyChanged("SystemError", false);
            }
        }

        public ICommand NewStepCommand
        {
            get { return new RelayCommand(p => this.BuildNextStep(true)); }
        }

        public ICommand RebuildCommand
        {
            get { return new RelayCommand(p => this.Rebuild()); }
        }

        private bool BuildDefinition()
        {
            if (this.definition != null)
            {
                return true;
            }

            if (this.definitionType == null)
            {
                return false;
            }

            this.definition = (LSystems.SystemDefinition)Activator.CreateInstance(this.definitionType);

            return this.definition != null;
        }        

        private bool InitSystem()
        {
            if (system != null)
            {                
                return true;
            }

            SystemError = null;

            if (!BuildDefinition())
            {                
                return false;
            }

            try
            {
                system = LSystems.SystemBuilder.BuildSystem(definition);
            }
            catch (InvalidOperationException e)
            {
                // Failure.
                SystemError = e.Message;
                return false;
            }

            LSystems.IRewriteRules rules = definition as LSystems.IRewriteRules;
            if (rules != null)
            {
                system.String = rules.Axiom;
            }            

            steps.Clear();
            this.steps.Insert(0, new Step() { System = system, Name = "Axiom" });

            return true;            
        }

        private void BuildNextStep(bool setCurrentItem)
        {
            if (!InitSystem())
            {
                return;
            }

            // Iterate.
            LSystems.IRewriteRules rules = definition as LSystems.IRewriteRules;
            if (rules != null)
            {
                LSystems.System nextStep = system.Clone();
                
                switch (rules.RewriteDirection)
                {
                    case LSystems.RewriteDirection.LeftToRight:
                        nextStep.RewriteLeftToRight();
                        break;

                    case LSystems.RewriteDirection.RightToLeft:
                        nextStep.RewriteRightToLeft();
                        break;

                    default:
                        throw new InvalidOperationException("Unknown RewriteDirection");
                }

                nextStep.Decomposite();

                this.steps.Insert(0, new Step() { System = nextStep, Name = string.Format("Step {0}", steps.Count) });

                system = nextStep;
            }

            if (setCurrentItem)
            {
                if (this.steps.Count > 0)
                {
                    CollectionViewSource.GetDefaultView(this.steps).MoveCurrentTo(this.steps[0]);
                }

                NotifyPropertyChanged("System");
            }            
        }

        public void Build()
        {
            if (system == null)
            {
                if (InitSystem())
                {
                    LSystems.IRewriteRules rules = definition as LSystems.IRewriteRules;
                    if (rules != null)
                    {
                        for (int i = 0; i < rules.Depth; ++i)
                        {
                            BuildNextStep(i == rules.Depth - 1);
                        }
                    }
                }
            }
        }

        public void Rebuild()
        {
            system = null;
            definition = null;
            steps.Clear();

            Build();
        }
    }
}
