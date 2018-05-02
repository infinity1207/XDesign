using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace XDesign
{
    public class ToolboxItem : ContentControl
    {
        private Point? _dragStartPoint;

        static ToolboxItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            _dragStartPoint = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _dragStartPoint = null;
            }

            if (_dragStartPoint.HasValue)
            {
                Point position = e.GetPosition(this);
                if ((SystemParameters.MinimumHorizontalDragDistance <=
                    Math.Abs(position.X - _dragStartPoint.Value.X)) ||
                    (SystemParameters.MinimumVerticalDragDistance <=
                    Math.Abs(position.Y - _dragStartPoint.Value.Y)))
                {
                    string xamlString = XamlWriter.Save(Content);
                    DataObject dataObject = new DataObject("DESIGNER_ITEM", xamlString);

                    DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
                }

                e.Handled = true;
            }
        }
    }
}