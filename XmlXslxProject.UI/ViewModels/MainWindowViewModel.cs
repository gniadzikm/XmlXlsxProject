using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using XmlXlsxProject.BusinessLogic;
using XmlXlsxProject.BusinessLogic.Interfaces;
using XmlXlsxProject.Models;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System;

namespace XmlXslxProject.UI.ViewModels
{
    internal class MainWindowViewModel : BaseUpdatableModel, IMainWindowViewModel
    {
        private string _fileName = string.Empty;
        private string _saveFileName = string.Empty;
        private Produkty _produkty = new Produkty();
        private ObservableCollection<ZdjeciePobrane> _zdjeciaPobrane = new ObservableCollection<ZdjeciePobrane>();
        private long _currentProgress;
        private bool _removeHtml = true;
        private IXmlXlsxBusinessLogic _xmlXlsxBusinessLogic = new XmlXlsxProjectBusinessLogic();
        private bool _controlsEnabled = true;

        public MainWindowViewModel()
        {
            _xmlXlsxBusinessLogic.ReportProgress += (sender, args) =>
            {
                CurrentProgress = args.Progress;
            };
        }

        public string FileName
        {
            set
            {
                _fileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OperationsEnabled));
            }
            get => _fileName;
        }

        public ObservableCollection<Produkt> Produkty
        {
            set
            {
                _produkty.ListaProduktow = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MaxProgress));
            }
            get => _produkty.ListaProduktow;
        }

        public ObservableCollection<ZdjeciePobrane> ZdjeciaPobrane
        {
            set
            {
                _zdjeciaPobrane = value;
                OnPropertyChanged();
            }
            get => _zdjeciaPobrane;
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
            get
            {
                int currentCount = _produkty.ListaProduktow.Count;
                return currentCount != 0 ? currentCount : 100;
            }
        }

        public bool ControlsEnabled
        {
            get => _controlsEnabled;
            set
            {
                _controlsEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(OperationsEnabled));
            }
        }

        public bool RemoveXml
        {
            set
            {
                _removeHtml = value;
                OnPropertyChanged();
            }
            get => _removeHtml;
        }

        public bool OperationsEnabled
        {
            get => _controlsEnabled && !string.IsNullOrWhiteSpace(_fileName);
        }

        private async void ProcessFile()
        {
            await Task.Run(() =>
            {
                ControlsEnabled = false;
                Produkty? produkty = null;

                if (!_xmlXlsxBusinessLogic.ProcessFiles(ref produkty, FileName, _removeHtml))
                {
                    MessageBox.Show($"Error while processing the file: {FileName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (produkty == null)
                {
                    return;
                }

                Produkty = new ObservableCollection<Produkt>(produkty.ListaProduktow);

                ControlsEnabled = true;
                GC.Collect();
            });
        }

        public async void GetFile()
        {
            await Task.Run(() =>
            {
                ControlsEnabled = false;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Xml files (*.xml)|*.xml|All files(*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    FileName = openFileDialog.FileName;
                }

                ControlsEnabled = true;
                GC.Collect();
            });
        }

        public async void SaveFile()
        {
            if (IsFileNotProcessed())
            {
                MessageBox.Show("Please process the file first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await Task.Run(() =>
            {
                ControlsEnabled = false;
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                if (saveFileDialog.ShowDialog() == true)
                {
                    _saveFileName = saveFileDialog.FileName;
                    if (!_xmlXlsxBusinessLogic.SaveFile(_produkty, _zdjeciaPobrane, _saveFileName, _removeHtml))
                    {
                        MessageBox.Show($"Error while saving file: {FileName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                    {
                        MessageBox.Show("File saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.None);
                    }
                }

                GC.Collect();
                CurrentProgress = 0;
                ControlsEnabled = true;
            });
        }

        public async void DownloadFiles()
        {
            if (IsFileNotProcessed())
            {
                MessageBox.Show("Please process the file first", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ControlsEnabled = false;
            ZdjeciaPobrane = await _xmlXlsxBusinessLogic.DownloadFiles(_produkty);
            MessageBox.Show("Download finished!", "Success", MessageBoxButton.OK, MessageBoxImage.None);
            CurrentProgress = 0;
            ControlsEnabled = true;
            GC.Collect();
        }

        public void ClearData()
        {
            FileName = string.Empty;
            _saveFileName = string.Empty;
            Produkty.Clear();
            ZdjeciaPobrane.Clear();
            CurrentProgress = 0;
            GC.Collect();
        }

        public void DeleteRow(long Id)
        {
            Produkt? tempProdukt = Produkty.FirstOrDefault(p => p.Id == Id);

            if (tempProdukt == null) return;

            Produkty.Remove(tempProdukt);
            OnPropertyChanged(nameof(MaxProgress));
            OnPropertyChanged(nameof(OperationsEnabled));
        }

        private bool IsFileNotProcessed()
        {
            return _produkty == null || !_produkty.ListaProduktow.Any();
        }

        public ICommand ProcessFileCommand => new RelayCommand(ProcessFile);
        public ICommand GetFileCommand => new RelayCommand(GetFile);
        public ICommand SaveFileCommand => new RelayCommand(SaveFile);
        public ICommand DownloadFilesCommand => new RelayCommand(DownloadFiles);
        public ICommand ClearDataCommand => new RelayCommand(ClearData);
        public ICommand DeleteRowCommand => new RelayCommand<long>(DeleteRow);
    }
}
