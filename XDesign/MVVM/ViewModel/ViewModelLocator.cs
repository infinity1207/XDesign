using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace XDesign.MVVM.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<JobViewModel>();
        }

        public static JobViewModel Job => ServiceLocator.Current.GetInstance<JobViewModel>();
    }
}
