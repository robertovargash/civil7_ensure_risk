using System.Data;

namespace DataMapping.Data
{
    public class DT_WBS:DataTable
    {
        public const string TABLE_NAME = "WBS";

        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string WBS_FNAME = "WBSFName";
        public const string NIVEL = "Nivel";
        public const string FNIVEL = "FNivel";
        public const string IDPROJECT = "idProject";
        //public const string IS_MANAGER = "isManager";
        public const string USERNAME = "userName";

        public DT_WBS() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_WBS, typeof(int));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(WBS_NAME, typeof(string));
            this.Columns.Add(WBS_FNAME, typeof(string));
            this.Columns.Add(NIVEL, typeof(string));
            this.Columns.Add(FNIVEL, typeof(string));
            this.Columns.Add(IDPROJECT, typeof(int));
            //this.Columns.Add(IS_MANAGER, typeof(bool));
            this.Columns.Add(USERNAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
