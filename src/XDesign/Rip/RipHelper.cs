using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;
using XDesign.MVVM.ViewModel;
using NLog;

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

        public Logger Logger => ViewModelLocator.Logger;

        public int XDpi { get; } = 600;

        public int YDpi { get; } = 300;

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

        void SaveBitmapSource(BitmapSource bs, string path)
        {
            var e = new TiffBitmapEncoder();
            e.Compression = TiffCompressOption.Default;
            e.Frames.Add(BitmapFrame.Create(bs));
            using (var s = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                e.Save(s);
            }
        }

        [DllImport("NativeUtil.dll")]
        private static extern void Pbgra32To1Bit(IntPtr src, long srcStride, long width, long height, IntPtr dest, int destStride, int leftPadding);

        void Pbgra32To1BitHelper(RenderTargetBitmap bs, int paddingLeft)
        {
            int bitsPerPixel = 1;

            var srcStride  = bs.PixelWidth * 4;
            int destStride = (bs.PixelWidth * bitsPerPixel + 31) / 32 * 4;

            int srcSize = srcStride * bs.PixelHeight;
            int destSize = destStride * bs.PixelHeight;

            var pSrc = Marshal.AllocHGlobal(srcSize);
            bs.CopyPixels(Int32Rect.Empty, pSrc, srcSize, srcStride);

            var pDest = Marshal.AllocHGlobal(destSize);

            Pbgra32To1Bit(pSrc, bs.PixelWidth, bs.PixelHeight, srcStride, pDest, destStride, paddingLeft);

            var bitmap = BitmapSource.Create(bs.PixelWidth, bs.PixelHeight, 600f, 300f, PixelFormats.Indexed1, BitmapPalettes.BlackAndWhite, pDest, destSize, destStride);
            SaveBitmapSource(bitmap, @"d:\\abc.tiff");

            Marshal.FreeHGlobal(pDest);
            Marshal.FreeHGlobal(pSrc);
        }

        void RipTextElement(TextElement textElement)
        {
            var ratioX = XDpi / 96f;
            var ratioY = YDpi / 96f;

            int pixelX = Convert.ToInt32(ratioX * textElement.Bound.X);
            int pixelY = Convert.ToInt32(ratioY * textElement.Bound.Y);
            int pixelW = Convert.ToInt32(ratioX * textElement.Bound.Width);
            int pixelH = Convert.ToInt32(ratioY * textElement.Bound.Height);

            int paddingLeft = pixelX % 8;

            DrawingVisual drawingVisual = new DrawingVisual();
            var dc = drawingVisual.RenderOpen();

            dc.PushTransform(new TranslateTransform(textElement.Bound.X * - 1, textElement.Bound.Y * -1));
            textElement.Draw(dc);
            dc.Pop();
            dc.Close();

            var r = new RenderTargetBitmap(pixelW, pixelH, XDpi, YDpi, PixelFormats.Pbgra32);
            r.Render(drawingVisual);

            Pbgra32To1BitHelper(r, paddingLeft);

            //SaveBitmapSource(r, @"d:\text.tiff");
        }

        void RipPage(int pageIndex)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

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
                var iDataBinding = element as IDataBinding;

                if (iDataBinding != null)
                    iDataBinding.DataIndex = pageIndex;

                if (element.Type == ElementType.Text)
                {
                    watch.Restart();
                    RipTextElement(element as TextElement);
                    watch.Stop();
                    Logger.Debug($"RipTextElement use {watch.ElapsedMilliseconds} ms");
                    //element.Draw(dc);
                }
            }
            dc.Close();

            var r = new RenderTargetBitmap(w, h, XDpi, YDpi, PixelFormats.Pbgra32);
            r.Render(drawingVisual);

            watch.Stop();
            Logger.Debug($"Rip page render text element use {watch.ElapsedMilliseconds} ms");
            watch.Restart();

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

            watch.Stop();
            Logger.Debug($"Rip page render barcode element use {watch.ElapsedMilliseconds} ms");
            watch.Restart();

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
            e.Compression = TiffCompressOption.Default;
            e.Frames.Add(BitmapFrame.Create(bs));

            var f = string.Format(@"d:\{0:D8}.tiff", pageIndex + 1);
            using (var s = new FileStream(f, FileMode.OpenOrCreate, FileAccess.Write))
            {
                e.Save(s);
            }

            watch.Stop();
            Logger.Debug($"Rip page save tiff use {watch.ElapsedMilliseconds} ms");
        }

        public void Perform()
        {
            for (int i = 0; i < Job.DataSource.Count; i++)
            {
                RipPage(i);
            }
        }
    }
}
