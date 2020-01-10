using System.Data;

namespace EnsureRisk.Export.Trader
{
    public class DataSetTrader
    {
        protected DataSet SourceDataSet { get; set; }

        public DataSetTrader(DataSet dataSet)
        {
            SourceDataSet = dataSet;
        }
    }
}
