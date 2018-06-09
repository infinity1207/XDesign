using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using XDesign.DataSource;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseElement : ObservableObject, IElement
    {
        public static Logger Logger { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType Type { get; set; }

        private int _zOrder;
        [JsonProperty]
        public int ZOrder
        {
            get => _zOrder;
            set
            {
                if (_zOrder != value)
                {
                    _zOrder = value;
                    RaisePropertyChanged();
                }
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseRectangleElement : BaseElement
    {
        private Rect _bound;

        [JsonProperty]
        public Rect Bound
        {
            get => _bound;
            set
            {
                if (_bound != value)
                {
                    _bound = value;
                    RaisePropertyChanged();
                }
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseDataBindingElement : BaseRectangleElement, IDataBinding
    {
        private string _rawContent;

        [JsonProperty]
        public string RawContent 
        {
            get => _rawContent;
            set
            {
                if (_rawContent != value)
                {
                    _rawContent = value;

                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Display));
                }
            }
        }


        private string _display;
        public virtual string Display
        {
            get
            {
                if (DataSource != null)
                    UpdateData();
                else
                    _display = RawContent;

                return _display;
            }
        }

        public IDataSource DataSource { get; set; }

        public int DataIndex { get; set; }

        public void UpdateData()
        {
            var columns = DataSource.GetColumns();
            var record = DataSource.GetRecord(DataIndex);

            var rgx = new Regex(@"\{(.+)?\}");

            var sb = new StringBuilder(RawContent);
            var matchs = rgx.Matches(RawContent);

            for (var i = matchs.Count - 1; i >= 0; i--)
            {
                var m = matchs[i];
                var column = m.Groups[1].Value;
                if (record.ContainsKey(column))
                {
                    sb.Remove(m.Index, m.Length);
                    sb.Insert(m.Index, record[column]);
                }
            }

            _display = sb.ToString();
        }
    }
}
