﻿using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;

namespace XDesign.MVVM.ViewModel
{
    public partial class JobViewModel
    {
        const string DefaultExt = ".xdf";
        const string Filter = "X-Design files (*.xdf)|*.xdf";

        private IElement _selectedElement;
        public IElement  SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;
                RaisePropertyChanged();
            }
        }

        public void AddElement(IElement element)
        {
            // 设置ZOrder
            Job.Elements.Add(element);
        }

        public void RemoveElement(IElement element)
        {
            Job.Elements.Remove(element);
        }

    }
}
