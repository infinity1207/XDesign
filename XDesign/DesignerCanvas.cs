using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using XDesign.MVVM.Model;
using XDesign.MVVM.View;

namespace XDesign
{
    public class DesignerCanvas : Canvas
    {
        private Point? dragStartPoint = null;

        public IEnumerable<DesignerItem> SelectedItems
        {
            get
            {
                var selectedItems = from item in this.Children.OfType<DesignerItem>()
                                    where item.IsSelected == true
                                    select item;

                return selectedItems;
            }
        }

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                this.dragStartPoint = new Point?(e.GetPosition(this));
                this.DeselectAll();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            string xamlString = e.Data.GetData("DESIGNER_ITEM") as string;
            if (!String.IsNullOrEmpty(xamlString))
            {
                FrameworkElement content = XamlReader.Load(XmlReader.Create(new StringReader(xamlString))) as FrameworkElement;
                if (content != null)
                {
                    Point position = e.GetPosition(this);
                    var w = 200;
                    var h = 100;
                    var x = Math.Max(0, position.X - w / 2);
                    var y = Math.Max(0, position.Y - h / 2);

                    var element = ElementFactory.CreateElement(
                        ElementType.Barcode,
                        new Rect
                        {
                            X = x,
                            Y = y,
                            Width = w,
                            Height = h
                        });

                    var newItem = new DesignerItem()
                    {
                        DataContext = element
                    };

                    var binding = new Binding();
                    binding.Converter = new ElementVisualConverter();
                    newItem.SetBinding(ContentControl.ContentProperty, binding);

                    var leftBinding = new Binding();
                    leftBinding.Path = new PropertyPath("Bound.X");
                    newItem.SetBinding(Canvas.LeftProperty, leftBinding);

                    var topBinding = new Binding();
                    topBinding.Path = new PropertyPath("Bound.Y");
                    newItem.SetBinding(Canvas.TopProperty, topBinding);

                    //var widthBinding = new Binding();
                    //widthBinding.Path = new PropertyPath("Bound.Width");
                    //newItem.SetBinding(Canvas.WidthProperty, widthBinding);

                    //var heightBinding = new Binding();
                    //heightBinding .Path = new PropertyPath("Bound.Height");
                    //newItem.SetBinding(Canvas.HeightProperty, heightBinding);

                    this.Children.Add(newItem);

                    this.DeselectAll();
                    newItem.IsSelected = true;
                }

                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();
            foreach (UIElement element in Children)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }

            // add some extra margin
            size.Width += 10;
            size.Height += 10;
            return size;
        }
    }
}
