using System.Management;

namespace XDesign
{
    public class Const
    {
        private static int _deviceDpi;
        public static int DeviceDpi
        {
            get
            {
                if (_deviceDpi == 0)
                {
                    using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
                    {
                        using (ManagementObjectCollection moc = mc.GetInstances())
                        {
                            foreach (var o in moc)
                            {
                                var each = (ManagementObject) o;
                                _deviceDpi = int.Parse((each.Properties["PixelsPerXLogicalInch"].Value.ToString()));
                                break;
                            }

                        }
                    }
                }
                return _deviceDpi;
            }
        }

        public static float ScreenScale => DeviceDpi / 96f;
    }
}
