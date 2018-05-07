﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using XDesign.MVVM.Model;
using XDesign.MVVM.View.ElementVisual;
using TECIT.TBarCode;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace XDesign.MVVM.View
{
    public class ElementVisualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = value as BaseElement;
            if (element == null)
                return null;

            Control visual = null;

            switch (element.ElementType)
            {
                case ElementType.None:
                    break;
                case ElementType.Text:
                    visual = new TextVisual
                    {
                        DataContext = element
                    };
                    break;
                case ElementType.Barcode:
                    visual = new BarcodeVisual
                    {
                        DataContext = element
                    };
                    break;
                default:
                    break;
            }

            return visual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BarcodeConverterEx : IMultiValueConverter
    {
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

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var element = values[0] as BarcodeElement;
            Debug.Assert(element != null);

            Barcode tbarcode = new Barcode();
            var w = System.Convert.ToInt32(element.Bound.Width);
            var h = System.Convert.ToInt32(element.Bound.Height);
            tbarcode.BoundingRectangle = new System.Drawing.Rectangle(0, 0, w, h);
            tbarcode.Data = element.Data;
            tbarcode.BarcodeType = element.BarcodeType;

            using (var bitmap = tbarcode.DrawBitmap())
            {
                return Bitmap2BitmapSource(bitmap);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
