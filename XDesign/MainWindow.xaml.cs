using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using NLog;
using TECIT.TBarCode;
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
            Logger = NLog.LogManager.GetCurrentClassLogger();

            Logger.Debug("Hello World");

            DataContext = ViewModelLocator.Job;
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
}
