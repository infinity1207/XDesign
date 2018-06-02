using XDesign.DataSource;

namespace XDesign.MVVM.Model.Element
{
    public interface IDataBinding
    {
        string RawContent { get; set; }

        string Display { get; }

        IDataSource DataSource { get; set; }

        int DataIndex { get; set; }

        void UpdateData();
    }
}
