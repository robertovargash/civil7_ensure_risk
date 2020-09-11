using System.Data;

namespace DataMapping.Data
{
    public class DT_CounterM_Damage : DataTable
    {
        public const string TABLE_NAME = "CounterM_TopRisk";

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
        public const string STATUS = "Enabled";
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
            TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_COUNTERM, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(decimal));
            Columns.Add(COUNTERM_NAMESHORT, typeof(string));
            Columns.Add(TOP_RISK, typeof(string));
            Columns.Add(DAMAGE, typeof(string));
            Columns.Add(COLOR, typeof(string));
            Columns.Add(RISK_REDUCTION, typeof(decimal));
            Columns.Add(RISK, typeof(string));
            Columns.Add(GROUPENAME, typeof(string));
            Columns.Add(STATUS, typeof(bool));
            Columns.Add(VALUE, typeof(decimal));
            Columns.Add(ID_RISK_TREE, typeof(decimal));
            Columns.Add(IDRISK, typeof(decimal));
            Columns.Add(ID_GROUPE, typeof(decimal));
            Columns.Add(ID_WBS, typeof(decimal));
            Columns.Add(WBS_NAME, typeof(string));
            Columns.Add(USERNAME, typeof(string));
            PrimaryKey = PK_COLUMN;
        }
    }
}
