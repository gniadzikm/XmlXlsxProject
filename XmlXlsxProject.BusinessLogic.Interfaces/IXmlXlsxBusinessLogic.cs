using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlXlsxProject.Models;

namespace XmlXlsxProject.BusinessLogic.Interfaces
{
    public interface IXmlXlsxBusinessLogic
    {
        event EventHandler<ReportProgressEventArgs> ReportProgress;
        bool ProcessFiles(ref Produkty? produkty, string filepath, bool removeHtml);
        bool SaveFile(Produkty? produkty, ObservableCollection<ZdjeciePobrane> zdjeciaPobrane, string filepath, bool removeHtml);
        Task<ObservableCollection<ZdjeciePobrane>> DownloadFiles(Produkty? produkty);
    }
}
