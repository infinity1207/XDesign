using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NLog;
using TECIT.TBarCode;
using XDesign.DataSource;
using XDesign.MVVM.Model.Element;
using XDesign.MVVM.ViewModel;
using Neodynamic.SDK.Barcode;

namespace XDesign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Logger Logger { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Logger = ViewModelLocator.Logger;
            BaseElement.Logger = Logger;

            DataContext = ViewModelLocator.JobViewModel;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            BarcodeTypes.ItemsSource = new List<Symbology>
            {
                Symbology.Code39,
                Symbology.Code128,
                Symbology.DataMatrix,
                Symbology.QRCode
            };

            FontNames.ItemsSource = Common.Utility.GetSystemFonts();

            FontSizes.ItemsSource = new List<float>
            {
                4,5,6,7,8,9,10,12,14,16,18,20,24,28,32,40,48,56,64,72
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

    public class DataColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as IDataSource)?.GetColumns();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridColumnsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new ObservableCollection<string>();

            var columns = (value as IDataSource)?.GetColumns();
            foreach (var column in columns)
            {
                result.Add(column);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ds =  value as IDataSource;

            List<string[]> result = new List<string[]>();
            for (int i = 0; i < ds.Count; i++)
            {
                result.Add(ds.GetRow(i));
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
