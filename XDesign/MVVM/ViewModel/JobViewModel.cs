using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using XDesign.MVVM.Model;

namespace XDesign.MVVM.ViewModel
{
    public partial class JobViewModel : ViewModelBase
    {
        public Job Job { get; set; } = new Job();

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

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

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(Job, settings);
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            using (var sr = File.OpenText(path))
            {
                var json = sr.ReadToEnd();
                var job = JsonConvert.DeserializeObject<Job>(json, settings);

                Job.Elements.Clear();

                foreach (var element in job.Elements)
                {
                    Job.Elements.Add(element);
                    Messenger.Default.Send(element, "JoinElement");
                }
            }
        }
    }
}
