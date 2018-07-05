using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neodynamic.SDK.Barcode;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace XDesign.MVVM.Model.Element
{
    public class ElementHelper
    {
        public static BarcodeProfessional CreateBarcodeProfessional(BarcodeElement element)
        {
            BarcodeProfessional bp = new BarcodeProfessional();

            bp.Symbology = element.BarcodeType;

            bp.Code = element.Display;
            bp.BarcodeUnit = BarcodeUnit.Inch;

            //bp.Width = element.Bound.Width / 96f;
            //bp.Height = element.Bound.Height / 96f;

            bp.BarWidth = element.ModuleWidth;

            switch (bp.Symbology)
            {
                case Symbology.QRCode:
                    bp.QRCodeModuleSize = element.ModuleWidth;
                    break;
                case Symbology.DataMatrix:
                    bp.DataMatrixModuleSize = element.ModuleWidth;
                    break;
                default:
                    bp.BarWidth = element.ModuleWidth;
                    break;
            }

            return bp;
        }

        public static BitmapSource GetBarcdoeImage(BarcodeProfessional bp, float xDpi, float yDpi)
        {
            using (Bitmap bitmap = bp.GetBarcodeImage(xDpi, yDpi) as Bitmap)
            {
                return Common.Utility.Bitmap2BitmapSource(bitmap);
            }
        }

        public static BitmapSource GetBarcdoeImage(BarcodeElement element, float xDpi, float yDpi)
        {
            var bp = CreateBarcodeProfessional(element);
            return GetBarcdoeImage(bp, xDpi, yDpi);
        }

        public static byte[] GetBarcdoeImageData(BarcodeProfessional bp, float xDpi, float yDpi)
        {
            return bp.GetBarcodeImage(System.Drawing.Imaging.ImageFormat.Bmp, xDpi, yDpi);
        }

        public static byte[] GetBarcdoeImageData(BarcodeElement element, float xDpi, float yDpi)
        {
            var bp = CreateBarcodeProfessional(element);
            return GetBarcdoeImageData(bp, xDpi, yDpi);
        }
    }
}
