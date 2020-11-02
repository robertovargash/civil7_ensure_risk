using System;
using System.Data;

namespace DataMapping.Data
{
    public class DT_MyLog : DataTable
    {
        public const string TABLE_NAME = "MyLog";

        public const string ID = "idLog";
        public const string LOG = "logName";
        public const string LOG_TYPE = "type";
        public const string USERNAME = "userName";
        public const string SERVERTIME = "serverTime";
        public const string LOCALTIME = "localTime";

        public DT_MyLog(): base()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(LOG, typeof(string));
            this.Columns.Add(LOG_TYPE, typeof(int));
            this.Columns.Add(USERNAME, typeof(string));
            this.Columns.Add(SERVERTIME, typeof(DateTime));
            this.Columns.Add(LOCALTIME, typeof(DateTime));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
