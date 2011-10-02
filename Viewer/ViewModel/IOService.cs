using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Viewer.ViewModel
{
    public interface IOService
    {
        string OpenFileDialog();
        string ReadFileContent(string fileName);

        void SubscribeForFileChanges(string fileName, Action action);
    }
}
