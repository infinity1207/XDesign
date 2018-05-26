using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XDesign
{
    public class Ruler : FrameworkElement 
    {
        static readonly float PenThickness = 1 / Const.ScreenScale;
        static readonly float HalfOfPenThickness = PenThickness / 2;

        public Ruler()
        {
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            Pen blackPen = new Pen(Brushes.Black, PenThickness);
            blackPen.Freeze();

            GeometryGroup group = new GeometryGroup();

            var step = 20;
            var position = 0;
            
            while (position < ActualWidth)
            {
                var pt0 = new Point { X = position, Y = 0 };
                var pt1 = new Point { X = position, Y = ActualHeight };

                // WPF绘制一个像素的线时会出现模糊的情况
                // 1. 根据显示器的dpi设置PenThickness
                // 2. 使用GuideLine
                //    GuideLine的使用参见 https://www.cnblogs.com/AaronLu/archive/2009/11/13/1602332.html
                dc.PushGuidelineSet(new GuidelineSet(new double[] {position + HalfOfPenThickness, position - HalfOfPenThickness }, null));
                group.Children.Add(new LineGeometry(pt0, pt1));

                position += step;
            }

            RenderOptions.SetEdgeMode(group, EdgeMode.Aliased);
            dc.DrawGeometry(null, blackPen, group);
            dc.Pop();
        }
    }
}
