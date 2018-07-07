using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XDesign.MVVM.Model.Element;

namespace XDesign.Rip
{
    public interface IRipElment
    {
        int XDpi { get; set; }
        int YDpi { get; set; }
        BaseRectangleElement Element { get; set; }

        void Rip(IntPtr pageBuffer);
    }

    public abstract class BaseRipElement : IRipElment
    {
        public int XDpi { get; set; }
        public int YDpi { get; set; }
        public BaseRectangleElement Element { get; set; }

        public abstract void Rip(IntPtr pageBuffer);

    }

    public class RipTextRipElement : BaseRipElement
    {
        public override void Rip(IntPtr pageBuffer)
        {
            var textElement = Element as BaseRectangleElement;
            var ratioX = XDpi / 96f;
            var ratioY = YDpi / 96f;

            int pixelX = Convert.ToInt32(ratioX * textElement.Bound.X);
            int pixelY = Convert.ToInt32(ratioY * textElement.Bound.Y);
            int pixelW = Convert.ToInt32(ratioX * textElement.Bound.Width);
            int pixelH = Convert.ToInt32(ratioY * textElement.Bound.Height);

            int paddingLeft = pixelX % 8;

            DrawingVisual drawingVisual = new DrawingVisual();
            var dc = drawingVisual.RenderOpen();

            dc.PushTransform(new TranslateTransform(textElement.Bound.X * -1, textElement.Bound.Y * -1));
            textElement.Draw(dc);
            dc.Pop();
            dc.Close();

            var r = new RenderTargetBitmap(pixelW, pixelH, XDpi, YDpi, PixelFormats.Pbgra32);
            r.Render(drawingVisual);

            Pbgra32To1BitHelper(r, paddingLeft);
        }

        [DllImport("NativeUtil.dll")]
        private static extern void Pbgra32To1Bit(IntPtr src, long srcStride, long width, long height, IntPtr dest, int destStride, int leftPadding);
        protected void Pbgra32To1BitHelper(RenderTargetBitmap bs, int paddingLeft)
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

            Marshal.FreeHGlobal(pDest);
            Marshal.FreeHGlobal(pSrc);
        }
    }
}
