using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace XDesign
{
    public enum RulerDirection
    {
        Horizontal,
        Vertical
    }

    public enum RulerUnit
    {
        Cm,
        Inch,
        Pixel,
    }

    public abstract class Ruler : FrameworkElement 
    {
        protected static readonly float PenThickness = 1 / Const.ScreenScale;
        protected static readonly float HalfOfPenThickness = PenThickness / 2;
        protected static readonly Color LineColor = Color.FromArgb(255, 130, 144, 163);
        protected static readonly Color TextColor = Color.FromArgb(255, 65, 68, 84);
        protected static readonly double TextEmSize = 9.0f;
        protected static readonly Typeface TextTypeface = new Typeface("Verdana");

        public RulerUnit Unit
        {
            get { return (RulerUnit)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(RulerUnit), typeof(Ruler), new PropertyMetadata(RulerUnit.Cm));

        public int ZeroPostion
        {
            get { return (int)GetValue(ZeroPostionProperty); }
            set { SetValue(ZeroPostionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZeroPostion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZeroPostionProperty =
            DependencyProperty.Register("ZeroPostion", typeof(int), typeof(Ruler), new PropertyMetadata(0));


    }

    public class HRuler : Ruler
    {
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var rulerWidth = RenderSize.Width;
            var rulerHeight = RenderSize.Height;

            Pen linePen = new Pen(new SolidColorBrush(LineColor), PenThickness);
            linePen.Freeze();

            Brush textBrush = new SolidColorBrush(TextColor);
            textBrush.Freeze();

            GeometryGroup lineGroup = new GeometryGroup();

            int unitsPerScale = 2;      // 一刻度占用多少逻辑单位
            int divide = 4;             // 一刻度被几等分
            int pixelsPerUnit = Convert.ToInt32(Const.DeviceDpi / 2.54f);   // 一个逻辑单位占用像素数

            // 绘制刻度线
            var step = unitsPerScale * pixelsPerUnit;
            var position = ZeroPostion;
            int scale = 0;
            Point pt0, pt1;
            while (position < rulerWidth)
            {
                pt0 = new Point { X = position, Y = 0 };
                pt1 = new Point { X = position, Y = rulerHeight - 1 };

                // WPF绘制一个像素的线时会出现模糊的情况
                // 1. 根据显示器的dpi设置PenThickness
                // 2. 使用GuideLine
                //    GuideLine的使用参见 https://www.cnblogs.com/AaronLu/archive/2009/11/13/1602332.html
                dc.PushGuidelineSet(new GuidelineSet(new double[] { position + HalfOfPenThickness, position - HalfOfPenThickness }, null));
                lineGroup.Children.Add(new LineGeometry(pt0, pt1));

                // 刻度值文本
                var ft = new FormattedText(scale.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, TextTypeface, 9, textBrush);
                dc.DrawText(ft, new Point { X = pt0.X + 2, Y = 0 });

                var textDrawing = new GlyphRunDrawing();

                // 刻度内的等分线
                var gap = Convert.ToInt32(step / (divide * 1.0f));
                for (int i = 0; i < divide; i++)
                {
                    pt0.Offset(gap, 0);
                    pt1.Offset(gap, 0);

                    if ((i + 1) == divide / 2)      // 位于最中间的等分线高于其他等分线
                        pt0.Y = (RenderSize.Height - 1) - 10;
                    else
                        pt0.Y = (RenderSize.Height - 1) - 6;

                    dc.PushGuidelineSet(new GuidelineSet(new double[] { position + HalfOfPenThickness, position - HalfOfPenThickness }, null));
                    lineGroup.Children.Add(new LineGeometry(pt0, pt1));
                }

                position += step;
                scale += unitsPerScale;
            }
            RenderOptions.SetEdgeMode(lineGroup, EdgeMode.Aliased);
            dc.DrawGeometry(null, linePen, lineGroup);

            // 绘制标尺底线
            pt0 = new Point { X = 0, Y = rulerHeight - 1 };
            pt1 = new Point { X = rulerWidth - 1, Y = rulerHeight - 1 };
            dc.PushGuidelineSet(new GuidelineSet(null, new double[] { pt0.Y - HalfOfPenThickness, pt0.Y + HalfOfPenThickness }));
            dc.DrawLine(linePen, pt0, pt1);

            dc.Pop();
        }
    }

    public class VRuler : Ruler
    {
        GlyphRun CreateGlyphRun(string text, Point origin, Typeface typeface, double emSize)
        {
            GlyphTypeface glyphTypeface;
            TextTypeface.TryGetGlyphTypeface(out glyphTypeface);

            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];
            double totalWidth = 0;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                glyphIndexes[n] = glyphIndex;

                double width = glyphTypeface.AdvanceWidths[glyphIndex] * emSize;
                advanceWidths[n] = width;

                totalWidth += width;
            }

            origin.Y += glyphTypeface.Height * emSize;

            return new GlyphRun(glyphTypeface, 0, false, emSize, glyphIndexes, origin, advanceWidths, null, null, null, null, null, null);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var rulerLength = RenderSize.Height;
            var rulerHeight = RenderSize.Width;

            Pen linePen = new Pen(new SolidColorBrush(LineColor), PenThickness);
            linePen.Freeze();
            Brush textBrush = new SolidColorBrush(TextColor);
            textBrush.Freeze();

            GeometryGroup lineGroup = new GeometryGroup();

            DrawingGroup textDrawingGroup = new DrawingGroup();
            var textDrawingContext = textDrawingGroup.Open();

            textDrawingContext.PushTransform(new RotateTransform(-90));

            int unitsPerScale = 2;      // 一刻度占用多少逻辑单位
            int divide = 4;             // 一刻度被几等分
            int pixelsPerUnit = Convert.ToInt32(Const.DeviceDpi / 2.54f);   // 一个逻辑单位占用像素数

            // 绘制刻度线
            var step = unitsPerScale * pixelsPerUnit;
            var position = ZeroPostion;
            int scale = 0;
            Point pt0, pt1;
            while (position < rulerLength)
            {
                pt0 = new Point { X = 0, Y = position };
                pt1 = new Point { X = rulerLength - 1, Y = position };

                // WPF绘制一个像素的线时会出现模糊的情况
                // 1. 根据显示器的dpi设置PenThickness
                // 2. 使用GuideLine
                //    GuideLine的使用参见 https://www.cnblogs.com/AaronLu/archive/2009/11/13/1602332.html
                dc.PushGuidelineSet(new GuidelineSet(new double[] { position + HalfOfPenThickness, position - HalfOfPenThickness }, null));
                lineGroup.Children.Add(new LineGeometry(pt0, pt1));

                // 刻度值文本
                var text = scale.ToString();
                var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, TextTypeface, 9, textBrush);
                //dc.DrawText(ft, new Point { X = 2, Y = pt0.Y });

                //var glyphRun = CreateGlyphRun(text, new Point { X = 2, Y = pt0.Y }, TextTypeface, TextEmSize);
                //dc.DrawGlyphRun(textBrush, glyphRun);

                textDrawingContext.DrawText(ft, new Point((position + ft.Width + 2) * -1f, 0));

                //textDrawingContext.Pop();

                // 刻度内的等分线
                var gap = Convert.ToInt32(step / (divide * 1.0f));
                for (int i = 0; i < divide; i++)
                {
                    pt0.Offset(0, gap);
                    pt1.Offset(0, gap);

                    if ((i + 1) == divide / 2)      // 位于最中间的等分线高于其他等分线
                        pt0.X = (rulerHeight - 1) - 10;
                    else
                        pt0.X = (rulerHeight  - 1) - 6;

                    dc.PushGuidelineSet(new GuidelineSet(null, new double[] { pt0.Y - HalfOfPenThickness, pt0.Y + HalfOfPenThickness }));
                    lineGroup.Children.Add(new LineGeometry(pt0, pt1));
                }

                position += step;
                scale += unitsPerScale;
            }
            RenderOptions.SetEdgeMode(lineGroup, EdgeMode.Aliased);
            dc.DrawGeometry(null, linePen, lineGroup);

            // 绘制标尺底线
            pt0 = new Point { X = rulerHeight - 1, Y = 0};
            pt1 = new Point { X = rulerHeight  - 1, Y = rulerLength - 1 };

            dc.PushGuidelineSet(new GuidelineSet(new double[] { pt0.X + HalfOfPenThickness, pt0.X - HalfOfPenThickness }, null));
            dc.DrawLine(linePen, pt0, pt1);

            dc.Pop();

            //dc.PushTransform(new RotateTransform(90));
            //dc.PushTransform(new ScaleTransform(1f, -1f));
            //var text1 = "123123123123123";
            //var ft1 = new FormattedText(text1, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, TextTypeface, 40, textBrush);
            //dc.DrawText(ft1, new Point(1, 1));

            textDrawingContext.Close();
            dc.DrawDrawing(textDrawingGroup);
        }
    }
}
