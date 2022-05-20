using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlXlsxProject.Models;

namespace XmlXlsxProject.BusinessLogic.Interfaces
{
    public interface IXmlXlsxBusinessLogic
    {
        bool ProcessFiles(ref Produkty? produkty, string filepath);
    }
}
