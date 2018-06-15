using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XDesign.Common
{
    public class Utility
    {
        public static double ConvertDouble(int value)
        {
            return value * 1.0f;
        }

        public static double ConvertDouble(string value)
        {
            return double.Parse(value);
        }

        public static int ConvertInt(string value)
        {
            return int.Parse(value);
        }

        public static void Swap<T>(ref T t1, ref T t2)
        {
            var temp = t1;
            t1 = t2;
            t2 = temp;
        }

        public static IEnumerable<string> GetSystemFonts()
        {
            using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
            {
                return from ff in installedFontCollection.Families
                       where !string.IsNullOrWhiteSpace(ff.Name)
                       select ff.Name;
            }
        }

        public static BitmapSource Bitmap2BitmapSource(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
    }
}
