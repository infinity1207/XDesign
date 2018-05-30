using Newtonsoft.Json;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TextElement : BaseDataBindingElement
    {
        public TextElement()
        {
            Type = ElementType.Text;
        }
    }
}
