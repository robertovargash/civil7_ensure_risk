using System.Data;

namespace DataMapping.Data
{
    public class DT_CounterM_Damage : DataTable
    {
        public const string TABLENAME = "CounterM_TopRisk";

        public const string ID_COUNTERM = "idCounterM";
        public const string ID_DAMAGE = "idTopRisk";
        public const string TOP_RISK = "TopRisk";
        public const string DAMAGE = "Damage";
        public const string COLOR = "Color";
        public const string COUNTERM_NAMESHORT = "nameShort";
        public const string VALUE = "value";
        public const string ID_RISK_TREE = "idRiskTree";

        public const string RISK_REDUCTION = "RiskReduction";
        public const string GROUPENAME = "GroupeName";
        public const string RISK = "Risk";
        public const string STATUS = "Status";
        public const string IDRISK = "idRisk";
        public const string ID_GROUPE = "idGroupe";
        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string USERNAME = "userName";

        public DT_CounterM_Damage() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_COUNTERM, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(System.Int32));
            Columns.Add(COUNTERM_NAMESHORT, typeof(System.String));
            Columns.Add(TOP_RISK, typeof(System.String));
            Columns.Add(DAMAGE, typeof(System.String));
            Columns.Add(COLOR, typeof(System.String));
            Columns.Add(RISK_REDUCTION, typeof(System.Decimal));
            Columns.Add(RISK, typeof(System.String));
            Columns.Add(GROUPENAME, typeof(System.String));
            Columns.Add(STATUS, typeof(System.String));
            Columns.Add(VALUE, typeof(System.Decimal));
            Columns.Add(ID_RISK_TREE, typeof(System.Int32));
            Columns.Add(IDRISK, typeof(System.Int32));
            Columns.Add(ID_GROUPE, typeof(System.Int32));
            Columns.Add(ID_WBS, typeof(System.Int32));
            Columns.Add(WBS_NAME, typeof(System.String));
            Columns.Add(USERNAME, typeof(System.String));
            PrimaryKey = PK_COLUMN;
        }
    }
}
