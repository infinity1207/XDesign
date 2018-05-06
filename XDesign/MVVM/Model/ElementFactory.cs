using System.Windows;

namespace XDesign.MVVM.Model
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
                        Text = "X-Design"
                    };
                    break;
                case ElementType.Barcode:
                    element = new BarcodeElement
                    {
                        Bound = bound,
                        Data = "20091207"
                    };
                    break;
                default:
                    break;
            }

            return element;
        }
    }
}
