using System.Data;

namespace DataMapping.Data
{
    public class DT_CounterM: DataTable
    {
        public const string TABLE_NAME = "CounterM";

        public const string ID = "idCounterM";
        public const string ID_RISK = "idRisk";
        public const string ID_RISK_TREE = "idRiskTree";
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

        public DT_CounterM():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID, typeof(System.Int32));
            //PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = 200; PK_COLUMN[0].AutoIncrementStep = 1;
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ID_RISK_TREE, typeof(System.Int32));
            Columns.Add(ID_RISK, typeof(System.Int32));
            Columns.Add(NAMESHORT, typeof(System.String));
            Columns.Add(RISK_NAMESHORT, typeof(System.String));
            Columns.Add(DETAIL, typeof(System.String));
            Columns.Add(DIAGONAL, typeof(System.Boolean));
            Columns.Add(FROM_TOP, typeof(System.Boolean));
            Columns.Add(PROBABILITY, typeof(System.Decimal));
            Columns.Add(POSITION, typeof(System.Int32));
            Columns.Add(ENABLED, typeof(System.Boolean));
            Columns.Add(ID_GROUPE, typeof(System.Int32));
            Columns.Add(GROUPE_NAME, typeof(System.String));
            Columns.Add(ID_WBS, typeof(System.Int32));
            Columns.Add(WBS_NAME, typeof(System.String));
            Columns.Add(USER_NAME, typeof(System.String));

            PrimaryKey = PK_COLUMN;
        }
    }
}
