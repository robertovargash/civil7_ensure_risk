using System.Data;

namespace EnsureRisk.Export.Trader
{
    public class DataSetTrader
    {
        public DataSet SourceDataSet { get; set; }

        public DataSetTrader(DataSet dataSet)
        {
            SourceDataSet = dataSet;
        }
    }
}
