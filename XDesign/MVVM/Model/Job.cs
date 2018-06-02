using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using XDesign.DataSource;
using XDesign.MVVM.Model.Element;

namespace XDesign.MVVM.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Job
    {
        [JsonProperty(PropertyName = "Page")]
        public Page Page { get; set; } = new Page { Width=800, Height= 600 };

        [JsonProperty(PropertyName = "Elements")]
        public ObservableCollection<IElement> Elements = new ObservableCollection<IElement>();

        public IDataSource DataSource { get; set; }

        public event Action<IDataSource> DataSourceChanged;

        public void LoadDataSource(string path)
        {
            var db = new FileDataSource();
            db.Connect(path);
            DataSource = db;

            OnDataSourceChanged(DataSource);
        }

        protected virtual void OnDataSourceChanged(IDataSource obj)
        {
            
            foreach (var element in Elements)
            {
                if (element is BaseDataBindingElement)
                    (element as BaseDataBindingElement).DataSource = obj;
            }

            DataSourceChanged?.Invoke(obj);
        }

        public void AddElement(IElement element)
        {
            // 设置ZOrder
            Elements.Add(element);
            if (element is BaseDataBindingElement)
            {
                (element as BaseDataBindingElement).DataSource = DataSource;
            }
        }

        public void RemoveElement(IElement element)
        {
            Elements.Remove(element);
        }
    }
}
