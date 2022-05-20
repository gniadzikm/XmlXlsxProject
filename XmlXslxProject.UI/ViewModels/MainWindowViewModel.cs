using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using XmlXlsxProject.BusinessLogic;
using XmlXlsxProject.BusinessLogic.Interfaces;
using XmlXlsxProject.Models;

namespace XmlXslxProject.UI.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private string _fileName = string.Empty;
        private Produkty _produkty = new Produkty();
        private long _maxProgress = 100;
        private long _currentProgress;
        private IXmlXlsxBusinessLogic _xmlXlsxBusinessLogic = new XmlXlsxProjectBusinessLogic();

        public string FileName
        {
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
            get => _fileName;
        }

        public ObservableCollection<Produkt> Produkty
        {
            set
            {
                _produkty.ListaProduktow = value;
                OnPropertyChanged();
            }
            get => _produkty.ListaProduktow;
        }

        public long CurrentProgress
        {
            set
            {
                _currentProgress = value;
                OnPropertyChanged();
            }
            get => _currentProgress;
        }

        public long MaxProgress
        {
            set
            {
                _maxProgress = value;
                OnPropertyChanged();
            }
            get => _maxProgress;
        }

        private async void ProcessFile()
        {
            await Task.Run(() =>
            {
                Produkty? produkty = null;

                if (!_xmlXlsxBusinessLogic.ProcessFiles(ref produkty, FileName))
                {
                    MessageBox.Show("Error while processing the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (produkty == null)
                {
                    return;
                }

                Produkty = new ObservableCollection<Produkt>(produkty.ListaProduktow);
            });
        }

        public async void GetFile()
        {
            await Task.Run(() =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Xml files (*.xml)|*.xml|All files(*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    FileName = openFileDialog.FileName;
                }
            });
        }

        public ICommand ProcessFileCommand => new RelayCommand(ProcessFile);
        public ICommand GetFileCommand => new RelayCommand(GetFile);
    }
}
