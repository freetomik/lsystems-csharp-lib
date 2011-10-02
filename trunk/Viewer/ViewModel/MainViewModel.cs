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
        private List<string> compileErrors;
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
                if (this.fileName != value)
                {
                    this.fileName = value;
                    this.NotifyPropertyChanged("FileName");
                }                
            }
        }

        public List<string> CompileErrors
        {
            get 
            { 
                return this.compileErrors;
            }

            set
            {
                this.compileErrors = value;
                this.NotifyPropertyChanged("CompileErrors");
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
            string filename = ioService.OpenFileDialog();
            if (filename.Length > 0)
            {
                Load(filename);
            }
        }

        public void LoadSelf()
        {
            this.FileName = "Self";
            this.CompileErrors = null;
            this.ioService.SubscribeForFileChanges(string.Empty, null);
            Assembly currentAssembly = Assembly.GetCallingAssembly();
            ExtractSystemDefinitions(Assembly.GetCallingAssembly());
        }

        public void Load(string fileName)
        {
            this.FileName = fileName;
            this.ioService.SubscribeForFileChanges(fileName, this.Reload);

            Assembly assembly = Compile(fileName);
            if (assembly != null)
            {                
                this.CompileErrors = null;                
                ExtractSystemDefinitions(assembly);
            }            
        }

        private void Reload()
        {
            this.Dispatcher.BeginInvoke(new System.Threading.ThreadStart(delegate() { this.Load(this.FileName); }));
        }

        private void ExtractSystemDefinitions(Assembly assembly)
        {
            var newList = new ObservableCollection<SystemDefinitionViewModel>();            
            foreach (var t in assembly.GetTypes())
            {
                if (!t.IsAbstract && t.IsSubclassOf(typeof(LSystems.SystemDefinition)))
                {
                    newList.Add(new SystemDefinitionViewModel(t));
                }
            }

            this.SelectedType = null;  
            this.definitions.Clear();

            newList.OrderBy(p => p.Name).ToList().ForEach(p => this.definitions.Add(p));            

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
                this.CompileErrors = new List<string>() 
                {
                    "Specified file does not exist or empty."
                };                
                return null;                
            }

            // Initialize compiler options.
            CompilerParameters compilerParameters = new CompilerParameters()
            {     
                GenerateInMemory = true,
                TreatWarningsAsErrors = true               
            };

            // Check source for "// reference" statements        
            var reader = new System.IO.StringReader(source);
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;
                string pattern =
                    "\\s*//\\s+reference\\s+\"(?<path>[^\"]*)\"\\s*";
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
                var list = new List<string>();
                foreach (var err in results.Errors)
                {
                    list.Add(err.ToString());
                }

                this.CompileErrors = list;
                return null;
            }
            

            return results.CompiledAssembly;
        }        
    }
}
