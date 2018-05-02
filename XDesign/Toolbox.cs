using System.Windows;
using System.Windows.Controls;

namespace XDesign
{
    public class Toolbox : ItemsControl
    {
        public Size DefaultItemSize { get; set; } = new Size(65, 65);

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}

