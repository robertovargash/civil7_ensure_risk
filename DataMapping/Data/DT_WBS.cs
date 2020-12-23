using System.Data;

namespace DataMapping.Data
{
    public class DT_WBS:DataTable
    {
        public const string TABLE_NAME = "WBS";

        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        private const string wBS_FNAME = "WBSFName";
        public const string NIVEL = "Nivel";
        private const string fNIVEL = "FNivel";
        public const string IDPROJECT = "idProject";
        private const string uSERNAME = "userName";
        public const string ID_FATHER = "idFather"; 
        public const string USER_WBS = "User-WBS";

        public static string WBS_FNAME => wBS_FNAME;

        public static string FNIVEL => fNIVEL;

        public static string USERNAME => uSERNAME;

        public DT_WBS() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_WBS, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(WBS_NAME, typeof(string));
            this.Columns.Add(WBS_FNAME, typeof(string));
            this.Columns.Add(NIVEL, typeof(string));
            this.Columns.Add(FNIVEL, typeof(string));
            this.Columns.Add(IDPROJECT, typeof(decimal));
            this.Columns.Add(ID_FATHER, typeof(decimal));
            this.Columns.Add(USERNAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
