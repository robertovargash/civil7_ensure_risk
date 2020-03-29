using System.Data;

namespace DataMapping.Data
{
    public class DT_Clasificator : DataTable
    {
        public const string IDCLASIFICATOR = "idClasificator";
        public const string CLASIFICATOR = "Clasificator";
        public const string TABLE_NAME = "Clasificator";

        public DT_Clasificator():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(IDCLASIFICATOR, typeof(int));
            //PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = 200; PK_COLUMN[0].AutoIncrementStep = 1;           
            Columns.Add(CLASIFICATOR, typeof(string));
            PrimaryKey = PK_COLUMN;
        }
    }
}
