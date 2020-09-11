using System.Data;


namespace DataMapping.Data
{
    public class DT_Groupe : DataTable
    {
        public const string TABLE_NAME = "Groupe";

        public const string ID_GROUPE = "idGroupe";
        public const string GROUPE_NAME = "GroupeName";

        public DT_Groupe():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_GROUPE, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(GROUPE_NAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
