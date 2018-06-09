using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XDesign.MVVM.Model;

namespace XDesign.Rip
{
    public class RipHelper
    {
        //new RenderTargetBitmap();
        public Job Job { get; set; }

        public void Perform()
        {
            var pen = new Pen(Brushes.Black, 2);

            DrawingVisual drawingVisual = new DrawingVisual();

            var dc = drawingVisual.RenderOpen();
            dc.DrawLine(pen, new System.Windows.Point(10, 10), new System.Windows.Point(100, 100));
            dc.Close();

            var bitmap = new RenderTargetBitmap(500, 500, 600, 600, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
        }
    }
}
