using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDesign
{
    public class Const
    {
        public static float ScreenScale
        {
            get;
            set;
        }

        static Const()
        {
            ScreenScale = 1.25f;
        }
    }
}
