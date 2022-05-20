using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XmlXlsxProject.BusinessLogic.Interfaces;
using XmlXlsxProject.Models;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using ClosedXML.Excel;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace XmlXlsxProject.BusinessLogic
{
    public class XmlXlsxProjectBusinessLogic : IXmlXlsxBusinessLogic
    {
        public bool ProcessFiles(ref Produkty? produkty, string filepath, bool removeHtml)
        {
            try
            {
                produkty = DeserializeToObject<Produkty>(filepath);

                if (produkty != null && produkty.ListaProduktow.Any() && removeHtml)
                {
                    foreach(Produkt produkt in produkty.ListaProduktow)
                    {
                        produkt.DlugiOpis = Regex.Replace(produkt.DlugiOpis, "<[^>]+>", string.Empty);
                    }
                }
                return true;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error while deserializing file: {filepath}, Error: {ex.Message}");
            }

            return false;
        }

        bool IXmlXlsxBusinessLogic.SaveFile(Produkty? produkty, ObservableCollection<ZdjeciePobrane> zdjeciaPobrane, string filepath, bool removeHtml)
        {
            if (produkty == null) return false;

            IXLWorkbook wb = new XLWorkbook();
            IXLWorksheet wsDane = wb.Worksheets.Add("dane");
            IXLWorksheet wsZdjecia = wb.Worksheets.Add("zdjecia");

            for (int i = 0; i < produkty.ListaProduktow.Count; ++i)
            {
                AddDataRowAndFormatting(produkty.ListaProduktow[i], wsDane, removeHtml, i);
            }

            for (int i = 0; i < zdjeciaPobrane.Count; ++i)
            {
                AddPhotoRowAndFormatting(zdjeciaPobrane[i], wsZdjecia, i);
            }

            wb.SaveAs(filepath);

            return true;
        }

        public async Task<ObservableCollection<ZdjeciePobrane>> DownloadFiles(Produkty? produkty)
        {
            ObservableCollection<ZdjeciePobrane> result = new ObservableCollection<ZdjeciePobrane>();

            string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "PobraneZdjecia");

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            if (produkty == null) return result;

            HttpClient httpClient = new HttpClient();
            foreach(Produkt produkt in produkty.ListaProduktow)
            {
                ZdjeciePobrane zdjeciePobrane = new ZdjeciePobrane
                {
                    Id = produkt.Id
                };

                foreach(Zdjecie zd in produkt.Zdjecia.OrderBy(z => z.Pozycja))
                {
                    string tempFileName = Path.Combine(tempPath, Path.ChangeExtension(Path.GetFileName(Path.GetTempFileName()), Path.GetExtension(zd.URL)));
                    HttpResponseMessage responseMessage = await httpClient.GetAsync(zd.URL.Trim());

                    if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK) continue;

                    using (FileStream fs = File.Open(tempFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await responseMessage.Content.CopyToAsync(fs);
                    }

                    zdjeciePobrane.PhotoPathList.Add(tempFileName);
                }

                result.Add(zdjeciePobrane);
            }

            return result;
        }

        private T? DeserializeToObject<T>(string filepath) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using(StreamReader sr = new StreamReader(filepath))
            {
                return (T?)xmlSerializer.Deserialize(sr);
            }
        }

        private bool IsValidImage(Stream stream)
        {
            try
            {
                Image img2 = Image.FromStream(stream);
                img2.Dispose();
            } catch (Exception)
            {
                return false;
            }
            finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            return true;
        }

        private void AddDataRowAndFormatting(Produkt tempProdukt, IXLWorksheet wsDane, bool removeHtml, int rowIndicator)
        {
            wsDane.Cell(rowIndicator + 1, 1).Value = tempProdukt.Id;
            wsDane.Cell(rowIndicator + 1, 2).Value = tempProdukt.Nazwa;
            wsDane.Cell(rowIndicator + 1, 3).Value = removeHtml ? Regex.Replace(tempProdukt.DlugiOpis, "<[^>]+>", string.Empty) : tempProdukt.DlugiOpis;
            wsDane.Cell(rowIndicator + 1, 4).Value = tempProdukt.Waga;
            wsDane.Cell(rowIndicator + 1, 5).Value = tempProdukt.Kod;
            wsDane.Cell(rowIndicator + 1, 6).Value = tempProdukt.EAN;
            wsDane.Cell(rowIndicator + 1, 7).Value = tempProdukt.Status;
            wsDane.Cell(rowIndicator + 1, 8).Value = tempProdukt.Typ;
            wsDane.Cell(rowIndicator + 1, 9).Value = tempProdukt.CenaZewnetrznaHurt;
            wsDane.Cell(rowIndicator + 1, 10).Value = tempProdukt.CenaZewnetrzna;
            wsDane.Cell(rowIndicator + 1, 11).Value = tempProdukt.Vat;
            wsDane.Cell(rowIndicator + 1, 12).Value = tempProdukt.IloscWariantow;
            wsDane.Cell(rowIndicator + 1, 13).Value = tempProdukt.Zdjecia.Count;
            wsDane.Cell(rowIndicator + 1, 14).Value = tempProdukt.Marza;

            if (tempProdukt.Zdjecia.Count < 2)
            {
                wsDane.Range(wsDane.Cell(rowIndicator + 1, 1), wsDane.Cell(rowIndicator + 1, 14)).Style.Fill.BackgroundColor = XLColor.Red;
            }

            if (tempProdukt.Marza < 0.2M)
            {
                wsDane.Range(wsDane.Cell(rowIndicator + 1, 1), wsDane.Cell(rowIndicator + 1, 14)).Style.Fill.BackgroundColor = XLColor.Orange;
            }
        }

        private void AddPhotoRowAndFormatting(ZdjeciePobrane zdp, IXLWorksheet wsZdjecia, int rowIndicator)
        {
            wsZdjecia.Row(rowIndicator + 1).Height = 100;
            wsZdjecia.Cell(rowIndicator + 1, 1).Value = zdp.Id;

            for (int j = 1; j < zdp.PhotoPathList.Count; ++j)
            {
                wsZdjecia.Column(j + 2).Width = 100;
                using (FileStream fs = File.Open(zdp.PhotoPathList[j], FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (!IsValidImage(fs)) continue;
                    wsZdjecia.AddPicture(fs).MoveTo(wsZdjecia.Cell(rowIndicator + 1, j + 2)).Scale(0.1);
                }
            }
        }
    }
}
