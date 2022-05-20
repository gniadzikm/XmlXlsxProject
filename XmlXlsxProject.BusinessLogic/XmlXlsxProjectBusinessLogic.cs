using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlXlsxProject.BusinessLogic.Interfaces;
using XmlXlsxProject.Models;

namespace XmlXlsxProject.BusinessLogic
{
    public class XmlXlsxProjectBusinessLogic : IXmlXlsxBusinessLogic
    {
        public bool ProcessFiles(ref Produkty? produkty, string filepath)
        {
            try
            {
                produkty = DeserializeToObject<Produkty>(filepath);
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error while deserializing file: {filepath}, Error: {ex.Message}");
            }

            return false;
        }

        private T? DeserializeToObject<T>(string filepath) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using(StreamReader sr = new StreamReader(filepath))
            {
                return (T?)xmlSerializer.Deserialize(sr);
            }
        }
    }
}
