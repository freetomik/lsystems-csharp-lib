using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Viewer.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private string fileName;
        private string compileError;
        private ObservableCollection<SystemDefinitionViewModel> definitions = 
            new ObservableCollection<SystemDefinitionViewModel>();
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


        public void LoadSelf()
        {
            this.FileName = "Self";
            this.CompileError = string.Empty;

            Assembly currentAssembly = Assembly.GetCallingAssembly();

            ExtractSystemDefinitions(Assembly.GetCallingAssembly());
        }

        public void Load(string fileName)
        {
            this.FileName = fileName;
            this.CompileError = string.Empty;
            
            Assembly assembly = Compile(fileName);

            if (assembly != null)
            {
                ExtractSystemDefinitions(assembly);
            }
        }

        private void ExtractSystemDefinitions(Assembly assembly)
        {
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

        private Assembly Compile(string fileName)
        {
            // Check if file is present.
            if (!File.Exists(fileName))
            {
                this.CompileError = "Specified file does not exist.";
                return null;
            }

            // Read source from file
            string source = ReadFile(fileName);

            // Initialize compiler options.
            CompilerParameters compilerParameters = new CompilerParameters()
            {
                //GenerateExecutable = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = true,
                CompilerOptions = "/nowarn:1633"
            };

            // Check source for #pragma reference statements        
            StringReader reader = new StringReader(source);
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

        private static string ReadFile(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
