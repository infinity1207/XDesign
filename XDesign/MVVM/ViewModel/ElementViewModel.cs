using System.Collections.ObjectModel;
using XDesign.MVVM.Model;
using Newtonsoft.Json;

namespace XDesign.MVVM.ViewModel
{
    public partial class JobViewModel
    {
        const string DefaultExt = ".xdf";
        const string Filter = "X-Design files (*.xdf)|*.xdf";

        private BaseElement _selectedElement;
        public BaseElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;
                RaisePropertyChanged();
            }
        }

        public void AddElement(BaseElement element)
        {
            // 设置ZOrder
            Job.Elements.Add(element);
        }

        public void RemoveElement(BaseElement element)
        {
            Job.Elements.Remove(element);
        }

    }
}
