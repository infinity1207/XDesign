using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TECIT.TBarCode;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BarcodeElement : BaseDataBindingElement 
    {
        private BarcodeType _barcodeType;

        [JsonProperty]
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
            Type = ElementType.Barcode;
            BarcodeType = BarcodeType.Code39;
        }
    }
}
