using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDesign.DataSource
{
    public class DataSourceException : Exception
    {
        public DataSourceException(string message) : base(message)
        {
        }
    }
}
