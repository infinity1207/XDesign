using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using NLog;

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
        }
    }
}
