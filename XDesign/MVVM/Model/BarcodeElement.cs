using TECIT.TBarCode;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XDesign.MVVM.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BarcodeElement : BaseRectangleElement
    {
        private string _data;

        [JsonProperty(PropertyName = "Data")]
        public string Data
        {
            get => _data;
            set
            {
                if (_data != value)
                {
                    _data = value;
                    RaisePropertyChanged();
                }
            }
        }

        private BarcodeType _barcodeType;

        [JsonProperty(PropertyName = "BarcodeType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public BarcodeType BarcodeType
        {
            get => _barcodeType;
            set
            {
                if (_barcodeType != value)
                {
                    _barcodeType = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BarcodeElement()
        {
            ElementType = ElementType.Barcode;
            BarcodeType = BarcodeType.Code39;
        }
    }
}
