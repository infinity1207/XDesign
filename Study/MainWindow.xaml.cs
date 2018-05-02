using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Study
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var bd1 = (Border)contentControl.Template.FindName("bd1", contentControl);
            var cp1 = (ContentPresenter)contentControl.Template.FindName("cp1", contentControl);

            var bd2 = (Border)contentControl.ContentTemplate.FindName("bd2", cp1);
            var cp2 = (ContentPresenter)contentControl.ContentTemplate.FindName("cp2", cp1);

            PrintInfo(bd1, cp1, bd2, cp2, btn);
        }

        void PrintInfo(params FrameworkElement[] eles)
        {
            string s = "";
            foreach (var ele in eles)
                s += String.Format("{2}\r\nParent: {0}\r\nTemplatedParent: {1}\r\n\r\n", ele.Parent, ele.TemplatedParent, ele.Name);
            MessageBox.Show(s);
        }
    }
}
