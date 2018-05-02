using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace XDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class MoveThumb: Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var item = this.DataContext as Control;

            if (item != null)
            {
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);

                Canvas.SetLeft(item, left + e.HorizontalChange);
                Canvas.SetTop(item, top + e.VerticalChange);
            }
        }
    }

    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var item = this.DataContext as Control;
            double deltaVertical, deltaHorizontal;
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    deltaVertical = Math.Min(-e.VerticalChange, item.ActualHeight - item.MinHeight);
                    item.Height -= deltaVertical;
                    break;
                case VerticalAlignment.Top:
                    deltaVertical = Math.Min(e.VerticalChange, item.ActualHeight - item.MinHeight);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                    item.Height -= deltaVertical;
                    break;
                default:
                    break;
            }
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.HorizontalChange,
                        item.ActualWidth - item.MinWidth);
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    item.Width -= deltaHorizontal;
                    break;
                case HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.HorizontalChange,
                        item.ActualWidth - item.MinWidth);
                    item.Width -= deltaHorizontal;
                    break;
                default:
                    break;
            }
            e.Handled = true;
        }
    }

    // Create a host visual derived from the FrameworkElement class.
    // This class provides layout, event handling, and container support for
    // the child visual objects.
    public class MyVisualHost : FrameworkElement
    {
        // Create a collection of child visual objects.
        private VisualCollection _children;

        public MyVisualHost()
        {
            _children = new VisualCollection(this);
            _children.Add(CreateDrawingVisualRectangle());
            //_children.Add(CreateDrawingVisualText());
            //_children.Add(CreateDrawingVisualEllipses());

            // Add the event handler for MouseLeftButtonUp.
            //this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
        }

        // Create a DrawingVisual that contains a rectangle.
        private DrawingVisual CreateDrawingVisualRectangle()
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            // Retrieve the DrawingContext in order to create new drawing content.
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // Create a rectangle and draw it in the DrawingContext.
            Rect rect = new Rect(new System.Windows.Point(160, 100), new System.Windows.Size(320, 80));
            drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue, (System.Windows.Media.Pen)null, rect);

            // Persist the drawing content.
            drawingContext.Close();

            return drawingVisual;
        }
    }
}
