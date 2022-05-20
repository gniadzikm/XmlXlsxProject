using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace XmlXlsxProject.Models
{
    [XmlRoot(ElementName = "produkty")]
    public class Produkty
    {
        [XmlElement(ElementName = "produkt")]
        public ObservableCollection<Produkt> ListaProduktow { get; set; } = new ObservableCollection<Produkt>();
    }
}
