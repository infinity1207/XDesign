using System.Collections.Generic;
using System.IO;

namespace XDesign.DataSource
{
    public class FileDataSource : IDataSource
    {
        public int Count { get; set; }

        public string ConnectString { get; set; }

        private readonly List<string> _columns = new List<string>();
        private readonly List<string> _rows = new List<string>();

        public void Connect(string connectString)
        {
            ConnectString = connectString;

            using (var sr = File.OpenText(connectString))
            {
                // 文件第一行为Column
                var line = sr.ReadLine();
                if (line == null)
                    throw new DataSourceException("");

                var parts = line.Split(',');
                _columns.AddRange(parts);

                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;

                    _rows.Add(line);
                }
            }
        }

        public string[] GetColumns()
        {
            return _columns.ToArray();
        }

        public string[] GetRecord(int index)
        {
            return _rows[index].Split(',');
        }
    }
}
