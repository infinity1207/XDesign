using Newtonsoft.Json;

namespace XDesign.MVVM.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Page
    {
        [JsonProperty(PropertyName = "Width")]
        public float Width { get; set; }

        [JsonProperty(PropertyName = "Height ")]
        public float Height { get; set; }
    }
}
