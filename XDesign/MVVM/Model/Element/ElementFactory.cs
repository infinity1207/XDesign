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
                        Content = "X-Design"
                    };
                    break;
                case ElementType.Barcode:
                    element = new BarcodeElement
                    {
                        Bound = bound,
                        Content = "20091207"
                    };
                    break;
                default:
                    break;
            }

            return element;
        }
    }
}
