using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDesign.DataSource
{
    public interface IDataSource
    {
        int Count { get; }

        string ConnectString { get; set; }

        void Connect(string connectString);

        string[] GetColumns();

        string[] GetRow(int index);

        Dictionary<string, string> GetRecord(int index);
    }
}
