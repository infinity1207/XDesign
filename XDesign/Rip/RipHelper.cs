using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;

namespace XDesign.Rip
{
    public class RipCanvas : Canvas
    {
        public RipCanvas()
        {
            SnapsToDevicePixels = true;
            UseLayoutRounding = true;
        }
    }

    public class RipHelper
    {
        private Job _job;

        public Job Job { get => _job; set => _job = value; }

        public int XDpi { get; } = 600;

        public int YDpi { get; } = 600;

        private void CopyBarcodeImage(BitmapSource barcodeImage, int xPixel, int yPixel, byte[] pixels, int stride)
        {
            var barcodeStride = barcodeImage.PixelWidth * 4;
            var barcodePixels = new byte[barcodeStride * barcodeImage.PixelHeight];
            barcodeImage.CopyPixels(barcodePixels, barcodeStride, 0);

            for (int i = 0; i < barcodeImage.PixelHeight; i++)
            {
                Array.Copy(barcodePixels, i * barcodeStride, pixels, (i + yPixel) * stride + (xPixel * 4), barcodeStride);
            }
        }

        public void Perform()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            var ratioX = XDpi / 96f;
            var ratioY = YDpi / 96f;
            var w = Convert.ToInt32(ratioX * Job.Page.Width);
            var h = Convert.ToInt32(ratioY * Job.Page.Height);

            // 绘制Text成员
            var dc = drawingVisual.RenderOpen();
            dc.DrawRectangle(Brushes.White, null, new Rect { X = 0, Y = 0, Width = w, Height = h });
            foreach (var element in Job.Elements)
            {
                if (element.Type == ElementType.Text)
                {
                    element.Draw(dc);
                }
            }
            dc.Close();

            var r = new RenderTargetBitmap(w, h, XDpi, YDpi, PixelFormats.Pbgra32);
            r.Render(drawingVisual);

            // 合并Barcde成员的Image
            var stride = r.PixelWidth * 4;
            var pixels = new byte[stride * r.PixelHeight];
            r.CopyPixels(pixels, stride, 0);

            foreach (var element in Job.Elements)
            {
                if (element.Type == ElementType.Barcode)
                {
                    var barcodeElement = element as BarcodeElement;
                    var barcodeImage = ElementHelper.GetBarcdoeImage(barcodeElement, XDpi, YDpi);
                    var x = Convert.ToInt32(barcodeElement.Bound.X / 96f * XDpi);
                    var y = Convert.ToInt32(barcodeElement.Bound.Y / 96f * YDpi);
                    CopyBarcodeImage(barcodeImage, x, y, pixels, stride);
                }
            }

            BitmapSource bs = BitmapSource.Create(
                r.PixelWidth,
                r.PixelHeight,
                XDpi,
                YDpi,
                PixelFormats.Pbgra32,
                null,
                pixels,
                stride);

            var e = new TiffBitmapEncoder();
            e.Compression = TiffCompressOption.Ccitt4;
            e.Frames.Add(BitmapFrame.Create(bs));

            var f = @"d:\temp.tiff";
            using (var s = new FileStream(f, FileMode.OpenOrCreate, FileAccess.Write))
            {
                e.Save(s);
            }
        }
    }
}
