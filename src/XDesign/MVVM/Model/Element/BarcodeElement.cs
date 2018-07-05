using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Media.Imaging;
using Neodynamic.SDK.Barcode;
using System.Drawing;
using System;

namespace XDesign.MVVM.Model.Element
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BarcodeElement : BaseDataBindingElement
    {
        private Symbology _barcodeType;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Symbology BarcodeType
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

        private double _moduleWidth;
        [JsonProperty]
        public double ModuleWidth {
            get => _moduleWidth;
            set
            {
                if (Math.Abs(_moduleWidth - value) > double.Epsilon)
                {
                    _moduleWidth = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BarcodeElement()
        {
            Type = ElementType.Barcode;
            BarcodeType = Symbology.Code39;
            ModuleWidth = 0.0104167f;
        }

        public override void Draw(DrawingContext dc)
        {
            base.Draw(dc);

            BarcodeProfessional bp = ElementHelper.CreateBarcodeProfessional(this);

            using (Bitmap bitmap = bp.GetBarcodeImage(600, 600) as Bitmap)
            {
                var bs = Bitmap2BitmapSource(bitmap);

                double w =  bitmap.Width / 600f * 96;
                double h =  bitmap.Height / 600f * 96;
                double x = (Bound.Width - w) / 2f;
                double y = (Bound.Height - h) / 2f;

                dc.DrawImage(bs, new System.Windows.Rect { X = x, Y = y, Width = w, Height = h });
            }

            //using (var bitmap = bp.GetBarcodeImage(600f, 600f) as Bitmap)
            //{
            //    var bs = Bitmap2BitmapSource(bitmap);
            //    dc.DrawImage(bs, Bound);
            //}

            //Barcode tbarcode = new Barcode();
            //var scaleX = 600 / 96f;
            //var scaleY = 600 / 96f;
            //var w = System.Convert.ToInt32(Bound.Width * scaleX);
            //var h = System.Convert.ToInt32(Bound.Height * scaleY);

            //tbarcode.BoundingRectangle = new System.Drawing.Rectangle(0, 0, w, h);
            //tbarcode.Data = Display;
            //tbarcode.BarcodeType = BarcodeType;

            //using (var bitmap = tbarcode.DrawBitmap())
            //{
            //    var bs = Bitmap2BitmapSource(bitmap);
            //    dc.DrawImage(bs, Bound);
            //}
        }

        public BitmapSource Bitmap2BitmapSource(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
    }
}
