using System;
using System.Windows;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace XDesign.MVVM.Model
{
    public enum ElementType
    {
        None,
        Text,
        Barcode
    }

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class BaseElement : ObservableObject
    {
        [JsonProperty(PropertyName = "ElementType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementType ElementType { get; set; }

        private int _zOrder;
        [JsonProperty(PropertyName = "ZOrder")]
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

        [JsonProperty(PropertyName = "Bound")]
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
    public class TextElement : BaseRectangleElement
    {
        private string _text;
        [JsonProperty(PropertyName = "Text")]
        public string Text { get => _text; set => _text = value; }

        public TextElement()
        {
            ElementType = ElementType.Text;
        }
    }
}
