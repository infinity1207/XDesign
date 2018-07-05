using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using NLog;

namespace XDesign.MVVM.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<JobViewModel>();

            SimpleIoc.Default.Register(LogManager.GetCurrentClassLogger);
        }

        public static JobViewModel JobViewModel => ServiceLocator.Current.GetInstance<JobViewModel>();

        public static Logger Logger => ServiceLocator.Current.GetInstance<Logger>();
    }
}
