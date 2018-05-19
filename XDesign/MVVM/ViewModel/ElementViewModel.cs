using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using XDesign.MVVM.Model;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json.Linq;
using System;

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

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(()=>
                    {
                        var dlg = new OpenFileDialog();
                        dlg.DefaultExt = DefaultExt;
                        dlg.Filter = Filter;
                        var result = dlg.ShowDialog();
                        if (result ?? false)
                        {
                            Load(dlg.FileName);
                        }
                    });
                }
                return _openCommand;
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

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(Elements, settings);
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            using (var sr = File.OpenText(path))
            {
                var json = sr.ReadToEnd();
                var elements = JsonConvert.DeserializeObject<ObservableCollection<BaseElement>>(json, settings);

                Elements.Clear();

                foreach (var element in elements)
                {
                    Elements.Add(element);
                    Messenger.Default.Send(element, "JoinElement");
                }
            }
        }
    }
}
