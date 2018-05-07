using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using XDesign.MVVM.Model;

namespace XDesign.MVVM.ViewModel
{
    public class ElementViewModel : ViewModelBase
    {
        public ObservableCollection<BaseElement> Elements = new ObservableCollection<BaseElement>();

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
            Elements.Add(element);
        }

        public void RemoveElement(BaseElement element)
        {
            Elements.Remove(element);
        }
    }
}
