using System.Data;

namespace DataMapping.Data
{
    public class DT_Tablita : DataTable
    {
        public const string ID = "ID";
        public const string NAME = "name";
        public const string IDFATHER = "idFather";
        public const string TABLE_NAME = "Tablita";

        public DT_Tablita() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true;
            PK_COLUMN[0].AutoIncrementSeed = -100;
            PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(NAME, typeof(string));
            Columns.Add(IDFATHER, typeof(decimal));
            PrimaryKey = PK_COLUMN;
        }
    }
}
