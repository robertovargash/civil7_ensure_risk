using System.Data;

namespace DataMapping.Data
{
    public class DT_Operation : DataTable
    {
        public const string OPERATION_TABLE = "Operation";

        public const string OPERATION_COLUMN = "Operation";

        public const string ID_COLUM = "idOperation";

        public DT_Operation():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = OPERATION_TABLE;
            PK_COLUMN[0] = this.Columns.Add(ID_COLUM, typeof(decimal));
            this.Columns.Add(OPERATION_COLUMN, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }        
    }
}
