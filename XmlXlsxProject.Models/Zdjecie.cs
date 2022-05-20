using System.Xml.Serialization;

namespace XmlXlsxProject.Models
{
    public class Zdjecie
    {
        [XmlAttribute(AttributeName = "pozycja")]
        public long Pozycja { get; set; }

        [XmlText]
        public string URL { get; set; } = string.Empty;
    }
}

