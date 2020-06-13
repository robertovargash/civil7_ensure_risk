using System.Data;

namespace DataMapping.Data
{
    public class DT_Risk_Damages:DataTable
    {
        public const string TABLENAME = "Risk_TopRisk";

        public const string ID_RISK = "idRisk";
        public const string ID_DAMAGE = "idTopRisk";
        public const string TOP_RISK = "TopRisk";
        public const string DAMAGE = "Damage";
        public const string COLOR = "Color";
        public const string RISK_NAMESHORT = "nameShort";
        public const string VALUE = "value";
        public const string FATHER = "Father";
        public const string STATUS = "Enabled";
        public const string PROBABILITY = "probability";
        public const string ID_RISK_TREE = "idRiskTree";

        public const string GROUPE_NAME = "GroupeName";
        public const string ID_FATHER = "IDFather";
        public const string RISK_TREE = "RiskTree";
        public const string IS_ROOT = "isRoot";
        public const string ID_GROUPE = "idGroupe";
        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string USERNAME = "userName";

        public DT_Risk_Damages():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK, typeof(int));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(int));
            Columns.Add(RISK_NAMESHORT, typeof(string));
            Columns.Add(FATHER, typeof(string));
            Columns.Add(STATUS, typeof(bool));
            Columns.Add(PROBABILITY, typeof(decimal));
            Columns.Add(TOP_RISK, typeof(string));
            Columns.Add(DAMAGE, typeof(string));
            Columns.Add(COLOR, typeof(string));
            Columns.Add(GROUPE_NAME, typeof(string));
            Columns.Add(RISK_TREE, typeof(string));
            Columns.Add(IS_ROOT, typeof(bool));
            Columns.Add(VALUE, typeof(decimal));
            Columns.Add(ID_RISK_TREE, typeof(int));
            Columns.Add(ID_GROUPE, typeof(int));
            Columns.Add(ID_FATHER, typeof(int));
            Columns.Add(ID_WBS, typeof(int));
            Columns.Add(WBS_NAME, typeof(string));
            Columns.Add(USERNAME, typeof(string));
            PrimaryKey = PK_COLUMN;
        }       
    }
}
