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

    public class HalftoneResult : IDisposable
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int BitsPerPixel { get; set; }

        public IntPtr Pixel0 { get; set; }

        public int Stride => (Width * BitsPerPixel + 31) / 32 * 4;

        public void Dispose()
        {
            if (Pixel0 != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Pixel0);
            }
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
        private static extern void Pbgra32To1Bit(IntPtr src, long width, long height, long srcStride, IntPtr dest, int destStride, int leftPadding);

        [DllImport("NativeUtil.dll")]
        private static extern void Argb32To1Bit(IntPtr src, long width, long height, long srcStride, IntPtr dest, int destStride, int leftPadding);

        [DllImport("NativeUtil.dll")]
        private static extern void CopyPixels(
            IntPtr src,
            long srcStride,
            long srcWidth,
            long srcHeight,
            IntPtr dest,
            long destStride,
            long destWidth,
            long destHeight,
            long x,
            long y);

        HalftoneResult Pbgra32To1BitHelper(RenderTargetBitmap bs, int paddingLeft)
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

            Marshal.FreeHGlobal(pSrc);

            return new HalftoneResult
            {
                Width = bs.PixelWidth,
                Height = bs.PixelHeight,
                BitsPerPixel = bitsPerPixel,
                Pixel0 = pDest,
            };
        }

        void RipBarcodeElement(BarcodeElement barcodeElement, HalftoneResult hrPage)
        {
            int bitsPerPixel = 1;

            var bp = ElementHelper.CreateBarcodeProfessional(barcodeElement);

            using (var bitmap = bp.GetBarcodeImage(XDpi, YDpi) as System.Drawing.Bitmap)
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);


                var srcStride = bitmapData.Stride;
                int destStride = (bitmap.Width * bitsPerPixel + 31) / 32 * 4;

                int srcSize = srcStride * bitmapData.Width;
                int destSize = destStride * bitmapData.Height;

                var pDest = Marshal.AllocHGlobal(destSize);

                Argb32To1Bit(bitmapData.Scan0, bitmap.Width, bitmap.Height,srcStride, pDest, destStride, 0);

                var x = Convert.ToInt32(barcodeElement.Bound.X / 96f * XDpi);
                var y = Convert.ToInt32(barcodeElement.Bound.Y / 96f * YDpi);
                CopyPixels(pDest, destStride, bitmap.Width, bitmap.Height, hrPage.Pixel0, hrPage.Stride, hrPage.Width, hrPage.Height, x, y);

                bitmap.UnlockBits(bitmapData);
            }
        }

        void RipTextElement(TextElement textElement, HalftoneResult hrPage)
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

            using (var hr = Pbgra32To1BitHelper(r, paddingLeft))
            {
                CopyPixels(hr.Pixel0, hr.Stride, hr.Width, hr.Height, hrPage.Pixel0, hrPage.Stride, hrPage.Width, hrPage.Height, pixelX, pixelY);
            }
            
        }

        void RipPage(int pageIndex, HalftoneResult hrPage)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach (var element in Job.Elements)
            {
                var iDataBinding = element as IDataBinding;

                if (iDataBinding != null)
                    iDataBinding.DataIndex = pageIndex;

                if (element.Type == ElementType.Text)
                {
                    watch.Restart();

                    RipTextElement(element as TextElement, hrPage);

                    watch.Stop();
                    Logger.Debug($"RipTextElement use {watch.ElapsedMilliseconds} ms");
                }
                else if (element.Type == ElementType.Barcode)
                {
                    watch.Restart();

                    RipBarcodeElement(element as BarcodeElement, hrPage);

                    watch.Stop();
                    Logger.Debug($"RipBarcodeElement use {watch.ElapsedMilliseconds} ms");
                }
                
            }

            //var f = string.Format(@"d:\{0:D8}.tiff", pageIndex + 1);
            //var bitmap = BitmapSource.Create(hrPage.Width, hrPage.Height, XDpi, YDpi, PixelFormats.Indexed1, BitmapPalettes.BlackAndWhite, 
            //    hrPage.Pixel0, hrPage.Stride * hrPage.Height, hrPage.Stride);
            //SaveBitmapSource(bitmap, f);
        }

        public void Perform()
        {
            int bitsPerPixel = 1;

            var ratioX = XDpi / 96f;
            var ratioY = YDpi / 96f;
            var w = Convert.ToInt32(ratioX * Job.Page.Width);
            var h = Convert.ToInt32(ratioY * Job.Page.Height);

            int stride = (w * bitsPerPixel + 31) / 32 * 4;
            int size = stride * h;

            var hrPage = new HalftoneResult {
                Width = w,
                Height = h,
                BitsPerPixel = bitsPerPixel,
                Pixel0 = Marshal.AllocHGlobal(size)
            };
            for (int i = 0; i < Job.DataSource.Count; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                RipPage(i, hrPage);

                sw.Stop();
                Logger?.Debug($"Rip one page consume: {sw.Elapsed}");
            }

            hrPage.Dispose();
        }
    }
}
