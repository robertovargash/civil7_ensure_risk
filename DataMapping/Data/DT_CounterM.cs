using System.Data;

namespace DataMapping.Data
{
    public class DT_CounterM: DataTable
    {
        public const string TABLE_NAME = "CounterM";

        public const string ID = "idCounterM";
        public const string ID_RISK = "idRisk";
        private const string iD_RISK_TREE = "idRiskTree";
        public const string NAMESHORT = "nameShort";
        public const string RISK_NAMESHORT = "Risk";
        public const string DETAIL = "detail";        
        public const string DIAGONAL = "diagonal";
        public const string FROM_TOP = "fromTop";
        public const string PROBABILITY = "probability";
        public const string POSITION = "position";
        public const string ENABLED = "EnableIt";
        public const string ID_GROUPE = "idGroupe";
        public const string GROUPE_NAME = "GroupeName";
        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string USER_NAME = "userName";

        public static string ID_DIAGRAM => iD_RISK_TREE;

        public DT_CounterM():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID, typeof(decimal));
            //PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = 200; PK_COLUMN[0].AutoIncrementStep = 1;
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ID_DIAGRAM, typeof(decimal));
            Columns.Add(ID_RISK, typeof(decimal));
            Columns.Add(NAMESHORT, typeof(string));
            Columns.Add(RISK_NAMESHORT, typeof(string));
            Columns.Add(DETAIL, typeof(string));
            Columns.Add(DIAGONAL, typeof(bool));
            Columns.Add(FROM_TOP, typeof(bool));
            Columns.Add(PROBABILITY, typeof(decimal));
            Columns.Add(POSITION, typeof(int));
            Columns.Add(ENABLED, typeof(bool));
            Columns.Add(ID_GROUPE, typeof(decimal));
            Columns.Add(GROUPE_NAME, typeof(string));
            Columns.Add(ID_WBS, typeof(decimal));
            Columns.Add(WBS_NAME, typeof(string));
            Columns.Add(USER_NAME, typeof(string));

            PrimaryKey = PK_COLUMN;
        }
    }
}
