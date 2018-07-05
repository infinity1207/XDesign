using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows;
using System.Globalization;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TextElement : BaseDataBindingElement
    {
        public TextElement()
        {
            Type = ElementType.Text;
        }

        public override void Draw(DrawingContext dc)
        {
            base.Draw(dc);

            var TextTypeface = new Typeface(FontName);
            var textBrush = new SolidColorBrush(Colors.Black);
            var ft = new FormattedText(Display, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, TextTypeface, FontSize, textBrush);

            var origin = new Point();
            origin.X = Bound.Left + (Bound.Width - ft.Width) / 2;
            origin.Y = Bound.Top + (Bound.Height - ft.Height) / 2;

            dc.DrawText(ft, origin);
        }
    }
}
