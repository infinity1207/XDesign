using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using XDesign.MVVM.Model;
using XDesign.MVVM.View;
using XDesign.MVVM.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using XDesign.MVVM.Model.Element;

namespace XDesign
{
    public class DesignerCanvas : Canvas
    {
        private Point? _dragStartPoint;

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

        public DesignerCanvas()
        {
            Messenger.Default.Register<IElement>(this, "JoinElement", (element) =>
            {
                AddChild(element);
            });
        }

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Delete)
            {
                var items = new List<DesignerItem>();
                foreach (var item in SelectedItems)
                {
                    items.Add(item);
                }

                foreach (var item in items)
                {
                    Children.Remove(item);
                    var element = item.DataContext as BaseElement;
                    Debug.Assert(element != null);
                    ViewModelLocator.JobViewModel.RemoveElement(element);
                }
            }

            e.Handled = true;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            Keyboard.Focus(this);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (Equals(e.Source, this))
            {
                this._dragStartPoint = new Point?(e.GetPosition(this));
                this.DeselectAll();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this._dragStartPoint = null;
            }

            if (this._dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, _dragStartPoint);
                    adornerLayer.Add(adorner);
                }

                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            Keyboard.Focus(this);

            string tag = e.Data.GetData("DESIGNER_ITEM") as string;
            if (!String.IsNullOrEmpty(tag))
            {
                if (Enum.TryParse<ElementType>(tag, out var elementType))
                {
                    Point position = e.GetPosition(this);
                    var w = 200;
                    var h = 100;
                    var x = Math.Max(0, position.X - w / 2);
                    var y = Math.Max(0, position.Y - h / 2);

                    var element = ElementFactory.CreateElement(
                        elementType,
                        new Rect
                        {
                            X = x,
                            Y = y,
                            Width = w,
                            Height = h
                        });

                    ViewModelLocator.JobViewModel.AddElement(element);

                    var newItem = AddChild(element);
                    this.DeselectAll();
                    newItem.IsSelected = true;
                }

                e.Handled = true;
            }
        }

        public DesignerItem AddChild(IElement element)
        {
            var newItem = new DesignerItem()
            {
                DataContext = element
            };

            var binding = new Binding { Converter = new ElementVisualConverter() };
            newItem.SetBinding(ContentControl.ContentProperty, binding);

            var leftBinding = new Binding { Path = new PropertyPath("Bound.X") };
            newItem.SetBinding(Canvas.LeftProperty, leftBinding);

            var topBinding = new Binding { Path = new PropertyPath("Bound.Y") };
            newItem.SetBinding(Canvas.TopProperty, topBinding);

            var widthBinding = new Binding { Path = new PropertyPath("Bound.Width") };
            newItem.SetBinding(Canvas.WidthProperty, widthBinding);

            var heightBinding = new Binding { Path = new PropertyPath("Bound.Height") };
            newItem.SetBinding(Canvas.HeightProperty, heightBinding);

            this.Children.Add(newItem);

            return newItem;
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
