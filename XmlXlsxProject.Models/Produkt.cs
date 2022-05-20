using System.Xml.Serialization;

namespace XmlXlsxProject.Models
{
    [XmlRoot(ElementName = "produkt")]
    public class Produkt
    {
        [XmlElement(ElementName = "id")]
        public long Id { get; set; }

        [XmlElement(ElementName = "nazwa")]
        public string Nazwa { get; set; } = string.Empty;

        [XmlElement(ElementName = "dlugi_opis")]
        public string DlugiOpis { get; set; } = string.Empty;

        [XmlElement(ElementName = "dane_techniczne")]
        public string DaneTechniczne { get; set; } = string.Empty;

        [XmlElement(ElementName = "waga")]
        public decimal Waga { get; set; }

        [XmlArray(ElementName = "zdjecia")]
        [XmlArrayItem(ElementName = "zdjecie")]
        public List<Zdjecie> Zdjecia { get; set; } = new List<Zdjecie>();

        [XmlElement(ElementName = "kod")]
        public string Kod { get; set; } = string.Empty;

        [XmlElement(ElementName = "ean")]
        public string EAN { get; set; } = string.Empty;

        [XmlElement(ElementName = "status")]
        public long Status { get; set; }

        [XmlElement(ElementName = "typ")]
        public string Typ { get; set; } = string.Empty;

        [XmlElement(ElementName = "cena_zewnetrzna_hurt")]
        public decimal CenaZewnetrznaHurt { get; set; }

        [XmlElement(ElementName = "cena_zewnetrzna")]
        public decimal CenaZewnetrzna { get; set; }

        [XmlElement(ElementName = "vat")]
        public decimal Vat { get; set; }

        [XmlElement(ElementName = "ilosc_wariantow")]
        public long IloscWariantow { get; set; }

        public int IloscZdjec
        {
            get => Zdjecia.Count;
        }

        [XmlIgnore]
        public decimal Marza
        {
            get => Math.Round((CenaZewnetrzna - CenaZewnetrznaHurt) / CenaZewnetrzna, 6);
        }

        [XmlIgnore]
        public bool LessThanTwoPictures
        {
            get => Zdjecia.Count < 2;
        }

        [XmlIgnore]
        public bool MarzaLessThan20
        {
            get => Marza < 0.20M;
        }
    }
}
