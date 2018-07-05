using Newtonsoft.Json;
using System.Windows.Media;

namespace XDesign.MVVM.Model.Element
{
    public enum ElementType
    {
        None,
        Text,
        Barcode
    }

    public interface IElement
    {
        ElementType Type { get; set; }

        int ZOrder { get; set; }

        void Draw(DrawingContext dc);
    }
}
