using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NLog;
using TECIT.TBarCode;
using XDesign.MVVM.Model;
using XDesign.MVVM.Model.Element;
using XDesign.MVVM.ViewModel;

namespace XDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Logger Logger { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Logger = ViewModelLocator.Logger;

            Logger.Debug("Hello World");

            DataContext = ViewModelLocator.JobViewModel;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            BarcodeTypes.ItemsSource = new List<BarcodeType>
            {
                BarcodeType.Code39,
                BarcodeType.Code128,
                BarcodeType.DataMatrix,
                BarcodeType.QRCode
            };
        }
    }

    public class ContentIsEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as IDataBinding) != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
