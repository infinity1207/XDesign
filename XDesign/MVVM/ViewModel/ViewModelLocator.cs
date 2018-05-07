using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace XDesign.MVVM.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ElementViewModel>();
        }

        public static ElementViewModel Element => ServiceLocator.Current.GetInstance<ElementViewModel>();
    }
}
