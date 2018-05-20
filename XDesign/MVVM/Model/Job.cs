using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace XDesign.MVVM.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Job
    {
        [JsonProperty(PropertyName = "Page")]
        public Page Page { get; set; } = new Page { Width=800, Height= 600 };

        [JsonProperty(PropertyName = "Elements")]
        public ObservableCollection<BaseElement> Elements = new ObservableCollection<BaseElement>();
    }
}
