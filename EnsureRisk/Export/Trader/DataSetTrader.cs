using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk.Export.Trader
{
    public class DataSetTrader
    {
        protected DataSet _sourceDataSet; 
        protected int _id;
        public DataSetTrader(DataSet dataSet, int id)
        {
            _sourceDataSet = dataSet;
            _id = id;
        }
    }
}
