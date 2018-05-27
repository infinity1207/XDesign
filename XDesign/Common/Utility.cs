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
    }
}
