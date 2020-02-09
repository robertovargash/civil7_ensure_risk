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
            PK_COLUMN[0] = this.Columns.Add(ID_WBS, typeof(System.Int32));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(WBS_NAME, typeof(System.String));
            this.Columns.Add(WBS_FNAME, typeof(System.String));
            this.Columns.Add(NIVEL, typeof(System.String));
            this.Columns.Add(FNIVEL, typeof(System.String));
            this.Columns.Add(IDPROJECT, typeof(System.Int32));
            //this.Columns.Add(IS_MANAGER, typeof(System.Boolean));
            this.Columns.Add(USERNAME, typeof(System.String));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
