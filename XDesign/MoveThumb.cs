﻿using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace XDesign
{
    public class MoveThumb : Thumb
    {
        private DesignerItem _designerItem;
        private DesignerCanvas _designerCanvas;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this._designerItem = DataContext as DesignerItem;

            if (this._designerItem != null)
            {
                this._designerCanvas = VisualTreeHelper.GetParent(this._designerItem) as DesignerCanvas;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this._designerItem != null && this._designerCanvas != null && this._designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                foreach (DesignerItem item in this._designerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                foreach (DesignerItem item in this._designerCanvas.SelectedItems)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                }

                this._designerCanvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
