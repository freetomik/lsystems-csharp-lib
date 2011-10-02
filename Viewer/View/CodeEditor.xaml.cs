using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Viewer.View
{
    /// <summary>
    /// Interaktionslogik für CodeEditor.xaml
    /// </summary>
    public partial class CodeEditor : Window
    {
        public CodeEditor()
        {
            this.Loaded += new RoutedEventHandler(CodeEditor_Loaded);            
            InitializeComponent();

            this.InputBindings.Add(new KeyBinding(
                new ViewModel.RelayCommand(prop => this.textEditor.Save(FileName)), 
                new KeyGesture(Key.S, ModifierKeys.Control)));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.DataContext != null)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        void CodeEditor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileName")
            {
                LoadFile(); 
            }
            else if (e.PropertyName == "CompileErrors")
            {
                CheckErrors();
            }
        }

        private string FileName
        {
            get { return ((ViewModel.MainViewModel)this.DataContext).FileName; }
        }

        private List<string> CompileErrors
        {
            get { return ((ViewModel.MainViewModel)this.DataContext).CompileErrors; }
        }

        private void LoadFile()
        {
            this.textEditor.Load(FileName);
        }

        void CodeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            this.textEditor.Options.ConvertTabsToSpaces = true;
            ((ViewModel.MainViewModel)this.DataContext).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CodeEditor_PropertyChanged);
            LoadFile();
            CheckErrors();
        }

        private void CheckErrors()
        {
            if (CompileErrors == null)
            {
                ErrorsListExpander.Header = "Compile OK";
                ErrorsListExpander.Background = Brushes.LightGreen;
                ErrorsListExpander.IsExpanded = false;
            }
            else
            {
                ErrorsListExpander.Header = "Compile Errors";
                ErrorsListExpander.Background = Brushes.LightSalmon;
                ErrorsListExpander.IsExpanded = true;
            }
        }

        void saveFileClick(object sender, EventArgs e)
        {
            this.textEditor.Save(FileName);
        }

        private void ErrorsListExpanderExpanded(object sender, RoutedEventArgs e)
        {
            this.textEditor.SetValue(Grid.RowSpanProperty, 1);
            this.ErrorsListExpander.SetValue(Grid.RowProperty, 2);
            this.ErrorsListExpander.SetValue(Grid.RowSpanProperty, 2);
            this.layoutRoot.UpdateLayout();
        }

        private void ErrorsListExpanderCollapsed(object sender, RoutedEventArgs e)
        {
            this.textEditor.SetValue(Grid.RowSpanProperty, 3);
            this.ErrorsListExpander.SetValue(Grid.RowProperty, 3);
            this.ErrorsListExpander.SetValue(Grid.RowSpanProperty, 1);
            this.layoutRoot.UpdateLayout();
        }
    }
}
