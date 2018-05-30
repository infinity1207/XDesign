using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;

namespace XDesign 
{
    public class ResizeThumb : Thumb
    {
        private DesignerItem _designerItem;
        private DesignerCanvas _designerCanvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this._designerItem = DataContext as DesignerItem;

            if (this._designerItem != null)
            {
                this._designerCanvas = VisualTreeHelper.GetParent(this._designerItem) as DesignerCanvas;
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this._designerItem != null && this._designerCanvas != null && this._designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                foreach (DesignerItem item in this._designerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, item.ActualHeight - item.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, item.ActualWidth - item.MinWidth);
                }

                foreach (DesignerItem item in this._designerCanvas.SelectedItems)
                {
                    BaseRectangleElement element = item.DataContext as BaseRectangleElement;
                    var bound = element.Bound;

                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            //item.Height = item.ActualHeight - dragDeltaVertical;
                            bound.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                        case VerticalAlignment.Top:
                            dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            Canvas.SetTop(item, Canvas.GetTop(item) + dragDeltaVertical);
                            //item.Height = item.ActualHeight - dragDeltaVertical;
                            bound.Height = item.ActualHeight - dragDeltaVertical;
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            Canvas.SetLeft(item, Canvas.GetLeft(item) + dragDeltaHorizontal);
                            //item.Width = item.ActualWidth - dragDeltaHorizontal;
                            bound.Width = item.ActualWidth - dragDeltaHorizontal;
                            
                            break;
                        case HorizontalAlignment.Right:
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            //item.Width = item.ActualWidth - dragDeltaHorizontal;
                            bound.Width = item.ActualWidth - dragDeltaHorizontal;
                            break;
                    }

                    element.Bound = bound;
                }

                e.Handled = true;
            }
        }
    }
}
