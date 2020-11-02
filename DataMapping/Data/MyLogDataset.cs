using System.Data;

namespace DataMapping.Data
{
    public class MyLogDataset : DataSet
    {
        public MyLogDataset()
        {
            Tables.Add(new DT_MyLog());
        }
    }
}
