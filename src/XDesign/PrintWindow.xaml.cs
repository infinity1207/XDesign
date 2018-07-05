using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XDesign.MVVM.ViewModel;
using Xyz.Pcs.DataType.DLLWrap;

namespace XDesign
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            int xDpi = 600;
            int yDpi = 300;

            int bitsPerPixel = 1;

            var ret = OnePassEngineDLL.Initialize();
            if (!ret)
                return;

            ret = OnePassEngineDLL.Wait_Ready(15 * 1000);
            if (!ret)
                return;

            var job = ViewModelLocator.JobViewModel.Job;

            var xPixels = Convert.ToInt32(job.Page.Width / 96f * xDpi);
            var yPixels = Convert.ToInt32(job.Page.Height / 96f * yDpi);
            int stride = (xPixels * bitsPerPixel + 31) / 32 * 4;

            int size = stride * yPixels;
            //byte[] buffer = new byte[size];

            IntPtr buffer = Marshal.AllocHGlobal(size);

            SJobEngineParam jobParam = new SJobEngineParam();
            jobParam.bitsPerPixel = bitsPerPixel;
            jobParam.colorCount = 1;
            jobParam.jobResolution_X = xDpi;
            jobParam.jobResolution_Y = yDpi;
            jobParam.pageCount = 4;

            var pJobHandle = OnePassEngineDLL.Print_Begin(ref jobParam, EJobType.Engine);

            for (int i = 0; i < jobParam.pageCount; i++)
            {
                if (OnePassEngineDLL.Print_IsAbort(pJobHandle))
                    break;

                if (!OnePassEngineDLL.Print_ColorData_ExistBuffer(pJobHandle, 100))
                    continue;

                SPageColorInfo pageInfo = new SPageColorInfo();
                pageInfo.bitsPerPixel = bitsPerPixel;
                pageInfo.bufferSize = size;
                pageInfo.bytesPerLine = stride;
                pageInfo.height = yPixels;
                pageInfo.width = xPixels;
                pageInfo.buffer = buffer;

                ret = OnePassEngineDLL.Print_ColorData_Add(pJobHandle, new SPageColorInfo[1] { pageInfo });
                if (!ret)
                    break;
            }

            ret = OnePassEngineDLL.Print_ColorData_End(pJobHandle);

            while (true)
            {
                if (OnePassEngineDLL.Print_WaitExit(pJobHandle, 100))
                    break;
            }

            ret = OnePassEngineDLL.Print_End(pJobHandle);

            ret = OnePassEngineDLL.UnInitialize();
            if (!ret)
                return;
        }
    }
}
