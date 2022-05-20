using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XmlXslxProject.UI.ViewModels
{
    public interface IMainWindowViewModel
    {
        void GetFile();
        void SaveFile();
        void DownloadFiles();
        public void ClearData();
    }
}
