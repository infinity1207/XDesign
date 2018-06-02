using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDesign.DataSource
{
    public interface IDataSource
    {
        int Count { get; set; }

        string ConnectString { get; set; }

        void Connect(string connectString);

        string[] GetColumns();

        string[] GetRecord(int index);
    }
}
