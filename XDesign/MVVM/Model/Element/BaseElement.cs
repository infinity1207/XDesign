using System.Windows;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseElement : ObservableObject, IElement
    {
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
        private string _context;
        [JsonProperty]
        public string Content
        {
            get => _context;
            set
            {
                if (_context != value)
                {
                    _context = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("Display");
                }
            }
        }

        public virtual string Display => Content;
    }

}
