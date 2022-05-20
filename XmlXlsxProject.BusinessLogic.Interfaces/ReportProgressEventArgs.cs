using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlXlsxProject.BusinessLogic.Interfaces
{
    public class ReportProgressEventArgs : EventArgs
    {
        public long Progress { get; set; }
    }
}
