using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Viewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private IOService ioService;
        private string fileName;
        private string compileError;
        private ObservableCollection<SystemDefinitionViewModel> definitions;
        private SystemDefinitionViewModel selectedType;


        public IEnumerable<SystemDefinitionViewModel> Types
        {
            get 
            {
                return this.definitions; 
            }
        }

        public string FileName
        {
            get 
            {
                return this.fileName;
            }

            private set
            {
                this.fileName = value;
                this.NotifyPropertyChanged("FileName");
            }
        }

        public string CompileError
        {
            get 
            { 
                return this.compileError;
            }

            set
            {
                this.compileError = value;
                this.NotifyPropertyChanged("CompileError");
            }
        }

        public SystemDefinitionViewModel SelectedType
        {
            get
            {
                return this.selectedType;
            }

            set
            {
                this.selectedType = value;

                if (this.selectedType != null)
                {
                    this.selectedType.Build();
                }

                NotifyPropertyChanged("SelectedType");
            }
        }

        public ICommand OpenFileCommand
        {
            get { return new RelayCommand(p => LoadFile()); }
        }        

        public MainViewModel(IOService ioService)
        {
            this.ioService = ioService;
            this.definitions = new ObservableCollection<SystemDefinitionViewModel>();
        }

        private void LoadFile()
        {           
            string filename = ioService.OpenFileDialog(System.IO.Directory.GetCurrentDirectory());
            if (filename.Length > 0)
            {
                Load(filename);
            }
        }

        public void LoadSelf()
        {
            this.FileName = "Self";
            this.CompileError = string.Empty;
            this.ioService.SubscribeForFileChanges(string.Empty, null);

            Assembly currentAssembly = Assembly.GetCallingAssembly();

            ExtractSystemDefinitions(Assembly.GetCallingAssembly());
        }

        public void Load(string fileName)
        {
            Assembly assembly = Compile(fileName);
            if (assembly != null)
            {
                this.FileName = fileName;
                this.CompileError = string.Empty;
                this.ioService.SubscribeForFileChanges(fileName, this.Reload);
            
                ExtractSystemDefinitions(assembly);
            }            
        }

        private void Reload()
        {
            this.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(delegate() { this.Load(this.FileName); }));
        }

        private void ExtractSystemDefinitions(Assembly assembly)
        {
            this.SelectedType = null;
            this.definitions.Clear();
            foreach (var t in assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(LSystems.SystemDefinition)))
                {
                    this.definitions.Add(new SystemDefinitionViewModel(t));
                }
            }

            this.SelectedType = this.definitions.Count > 0 ? this.definitions[0] : null;
        }

        // Compile source code mainly taken from Phil Trelford's Array, see
        // http://www.trelford.com/blog/post/C-Scripting.aspx
        private Assembly Compile(string fileName)
        {
            // Read source from file
            string source = ioService.ReadFileContent(fileName);
            if (source.Length <= 0)
            {                
                this.CompileError = "Specified file does not exist or empty.";
                return null;                
            }

            // Initialize compiler options.
            CompilerParameters compilerParameters = new CompilerParameters()
            {     
                GenerateInMemory = true,
                TreatWarningsAsErrors = true,
                CompilerOptions = "/nowarn:1633"
            };

            // Check source for #pragma reference statements        
            var reader = new System.IO.StringReader(source);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                string pattern =
                    "\\s*#pragma\\s+reference\\s+\"(?<path>[^\"]*)\"\\s*";
                Match match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    compilerParameters.ReferencedAssemblies.Add(
                        match.Groups["path"].Value);
                }
            }

            // Specify .NET version.
            var providerOptions = new Dictionary<string, string>();
            providerOptions.Add("CompilerVersion", "v3.5");
            
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);            
            
            // Compile source.
            CompilerResults results = provider.CompileAssemblyFromSource(
                compilerParameters, new string[] { source });

            // Show errors.
            if (results.Errors.HasErrors)
            {                
                StringBuilder sb = new StringBuilder();
                foreach (var err in results.Errors)
                {
                    sb.AppendLine(err.ToString());
                }

                this.CompileError = sb.ToString();
                return null;
            }
            

            return results.CompiledAssembly;
        }        
    }
}
