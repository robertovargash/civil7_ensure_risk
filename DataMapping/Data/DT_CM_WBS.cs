using System.Data;

namespace DataMapping.Data
{
    public class DT_CM_WBS: DataTable
    {
        public const string TABLE_NAME = "CM_WBS";

        private const string iD_CM = "idCM";
        private const string iD_WBS = "idWBS";
        private const string cM = "CM";
        private const string wBS = "WBS_Name";
        private const string nIVEL = "Nivel";
        private const string iS_PRIMARY = "isPrimary";
        private const string pRIMARY = "Primaryy";
        private const string uSERNAME = "userName";
        private const string pROBABILITY = "probability";
        private const string wBS_USER = "WBS_User";

        public static string WBS_USER => wBS_USER;

        public static string ID_CM => iD_CM;

        public static string ID_WBS => iD_WBS;

        public static string CM => cM;

        public static string WBS => wBS;

        public static string NIVEL => nIVEL;

        public static string IS_PRIMARY => iS_PRIMARY;

        public static string PRIMARY => pRIMARY;

        public static string USERNAME => uSERNAME;

        public static string PROBABILITY => pROBABILITY;

        public DT_CM_WBS() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_CM, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(ID_WBS, typeof(decimal));
            Columns.Add(CM, typeof(string));
            Columns.Add(WBS, typeof(string));
            Columns.Add(NIVEL, typeof(string));
            Columns.Add(IS_PRIMARY, typeof(bool));
            Columns.Add(PRIMARY, typeof(string));
            Columns.Add(USERNAME, typeof(string));
            Columns.Add(PROBABILITY, typeof(decimal));
            Columns.Add(WBS_USER, typeof(string));
            PrimaryKey = PK_COLUMN;
        }
    }
}
