using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECIT.TBarCode;

namespace XDesign.MVVM.Model
{
    public class BarcodeElement : BaseRectangleElement
    {
        private string _data;

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
