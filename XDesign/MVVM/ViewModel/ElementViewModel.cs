using System.Collections.ObjectModel;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using XDesign.MVVM.Model;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace XDesign.MVVM.ViewModel
{
    public class ElementViewModel : ViewModelBase
    {
        const string DefaultExt = ".xdf";
        const string Filter = "X-Design files (*.xdf)|*.xdf";

        public ObservableCollection<BaseElement> Elements = new ObservableCollection<BaseElement>();

        private BaseElement _selectedElement;

        public BaseElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;
                RaisePropertyChanged();
            }
        }

        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(() =>
                    {
                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.DefaultExt = DefaultExt;
                        dlg.Filter = Filter;
                        var result = dlg.ShowDialog();
                        if (result ?? false)
                        {
                            Save(dlg.FileName);
                        }
                    });
                }

                return _saveCommand;
            }
        }


        public void AddElement(BaseElement element)
        {
            // 设置ZOrder
            Elements.Add(element);
        }

        public void RemoveElement(BaseElement element)
        {
            Elements.Remove(element);
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(Elements);
            File.WriteAllText(path, json);
        }
    }
}
