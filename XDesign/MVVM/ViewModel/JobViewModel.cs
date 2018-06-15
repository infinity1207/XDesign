using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using NLog;
using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;
using XDesign.Rip;
using System.Diagnostics;

namespace XDesign.MVVM.ViewModel
{
    public partial class JobViewModel : ViewModelBase
    {
        public Logger Logger => ViewModelLocator.Logger;

        public Job Job { get; set; } = new Job();

        readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public JobViewModel()
        {
            LoadDataSource();
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

        private RelayCommand _ripCommand;
        public RelayCommand RipCommand
        {
            get
            {
                if (_ripCommand == null)
                {
                    _ripCommand = new RelayCommand(() =>
                    {
                        RipHelper ripHelper = new RipHelper { Job = Job};
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ripHelper.Perform();
                        sw.Stop();

                        Logger?.Debug($"Rip one page consume: {sw.Elapsed}");
                    });
                }

                return _ripCommand;
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
                        var dlg = new OpenFileDialog
                        {
                            DefaultExt = DefaultExt,
                            Filter = Filter
                        };
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

        private RelayCommand<string> _insertDataFieldCommand;
        public RelayCommand<string> InsertDataFieldCommand
        {
            get
            {
                if (_insertDataFieldCommand == null)
                {
                    _insertDataFieldCommand = new RelayCommand<string>((field) =>
                    {
                        var element = SelectedElement as IDataBinding;
                        if (element != null)
                        {
                            element.RawContent += string.Format("{{{0}}}", field);
                        }
                    });
                }

                return _insertDataFieldCommand;
            }
        }
        

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(Job, _settings);
            File.WriteAllText(path, json);
        }

        public void Load(string path)
        {
            using (var sr = File.OpenText(path))
            {
                var json = sr.ReadToEnd();
                var job = JsonConvert.DeserializeObject<Job>(json, _settings);

                Job.Elements.Clear();

                foreach (var element in job.Elements)
                {
                    Job.Elements.Add(element);
                    Messenger.Default.Send(element, "JoinElement");
                }

                LoadDataSource();
            }
        }

        public void LoadDataSource()
        {
            var path = Path.Combine(Environment.CurrentDirectory, @"db.txt");
            Job.LoadDataSource(path);
        }
    }
}
