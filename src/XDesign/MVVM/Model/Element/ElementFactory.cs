using System.Windows;

namespace XDesign.MVVM.Model.Element
{
    public class ElementFactory
    {
        public static BaseElement CreateElement(ElementType type, Rect bound)
        {
            BaseElement element = null;

            switch (type)
            {
                case ElementType.None:
                    break;
                case ElementType.Text:
                    element = new TextElement
                    {
                        Bound = bound,
                        RawContent = "X-Design"
                    };
                    break;
                case ElementType.Barcode:
                    element = new BarcodeElement
                    {
                        Bound = bound,
                        RawContent = "20171111"
                    };
                    break;
            }

            return element;
        }
    }
}
