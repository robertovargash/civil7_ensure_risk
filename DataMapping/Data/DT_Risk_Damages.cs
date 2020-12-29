using System.Data;

namespace DataMapping.Data
{
    public class DT_Risk_Damages:DataTable
    {
        public const string TABLE_NAME = "Risk_TopRisk";

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
        public const string iS_CM = "isCM";
        public const string CanEditName = "CanEditName";
        public const string CanEditProbability = "CanEditProbability";

        public static string IS_CM => iS_CM;


        public DT_Risk_Damages():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(decimal));
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
            Columns.Add(IS_CM, typeof(bool));
            Columns.Add(VALUE, typeof(decimal));
            Columns.Add(ID_RISK_TREE, typeof(decimal));
            Columns.Add(ID_GROUPE, typeof(decimal));
            Columns.Add(ID_FATHER, typeof(decimal));
            Columns.Add(ID_WBS, typeof(decimal));
            Columns.Add(WBS_NAME, typeof(string));
            Columns.Add(USERNAME, typeof(string));
            Columns.Add(CanEditName, typeof(bool));
            Columns.Add(CanEditProbability, typeof(bool));
            PrimaryKey = PK_COLUMN;
        }       
    }
}
