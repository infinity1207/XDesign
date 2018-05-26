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
    }
}
