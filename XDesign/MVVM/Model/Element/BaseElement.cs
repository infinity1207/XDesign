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
            string[] values = DataSource.GetRecord(DataIndex);

            Regex rgx = new Regex(@"\{(.+)?\}");

            _display = RawContent;
            foreach (Match m in rgx.Matches(RawContent))
            {
                ;
            }
        }
    }
}
