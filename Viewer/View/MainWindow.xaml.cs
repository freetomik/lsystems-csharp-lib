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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Viewer.ViewModel;
using System.IO;

namespace Viewer.View
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class MainWindow : Window, ViewModel.IOService
    {
        private string initialDirectory;

        public MainWindow()
        {
            InitializeComponent();

            initialDirectory = System.IO.Directory.GetCurrentDirectory();

            MainViewModel mainModel = new MainViewModel(this);
            mainModel.Load(@".\TestData\DragonCurve.cs");            
            this.DataContext = mainModel;            
        }        

        public string OpenFileDialog()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();            
            openFileDialog.Filter = "cs files (*.cs)|*.cs|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = initialDirectory;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog().Value)
            {
                initialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                return openFileDialog.FileName;
            }
            else
            {
                return string.Empty;
            }            
        }

        public string ReadFileContent(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }

            try
            {
                return File.ReadAllText(fileName);
            }            
            catch (System.IO.IOException)
            {
                return string.Empty;
            }            
        }

        FileSystemWatcher watch;        

        public void SubscribeForFileChanges(string fileName, Action action)
        {
            if (this.watch != null)
            {
                this.watch.EnableRaisingEvents = false;
                this.watch.Dispose();
                this.watch = null;                
            }

            if (fileName.Length == 0)
            {
                return;
            }

            string shortFileName = System.IO.Path.GetFileName(fileName);

            this.watch = new FileSystemWatcher();
            this.watch.Path = System.IO.Path.GetDirectoryName(fileName);
            this.watch.NotifyFilter = NotifyFilters.LastWrite;
            this.watch.Changed += new FileSystemEventHandler(
                delegate(object s, FileSystemEventArgs e)
                {
                    if (e.Name == shortFileName)
                    {
                        action();
                    }
                });
            this.watch.EnableRaisingEvents = true;
        }


        CodeEditor editor;

        private void EditClick(object sender, RoutedEventArgs e)
        {
            if (editor == null)
            {
                editor = new CodeEditor();
                editor.DataContext = this.DataContext;                
            }
            else
            {
                // Hide and show will brind the window on top
                editor.Hide();
            }

            editor.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.editor != null)
            {
                this.editor.DataContext = null;
                this.editor.Close();
            }
        }
    }
}
