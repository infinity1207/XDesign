using System.Windows;
using GalaSoft.MvvmLight;

namespace XDesign.MVVM.Model
{
    public enum ElementType
    {
        None,
        Text,
        Barcode
    }

    public abstract class BaseElement : ObservableObject
    {
        private ElementType _elementType;

        private int _zOrder;

        public ElementType ElementType { get => _elementType; set => _elementType = value; }

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

    public abstract class BaseRectangleElement : BaseElement
    {
        private Rect _bound;

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

    public class TextElement : BaseRectangleElement
    {
        private string _text;

        public string Text { get => _text; set => _text = value; }

        public TextElement()
        {
            ElementType = ElementType.Text;
        }
    }

}
