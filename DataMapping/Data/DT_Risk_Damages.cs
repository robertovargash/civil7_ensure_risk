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
        public const string STATUS = "Status";
        public const string PROBABILITY = "probability";
        public const string ID_RISK_TREE = "idRiskTree";

        public const string GROUPE_NAME = "GroupeName";
        public const string ID_FATHER = "IDFather";
        public const string RISK_TREE = "RiskTree";
        public const string IS_ROOT = "isRoot";
        public const string ID_GROUPE = "idGroupe";

        public DT_Risk_Damages():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(System.Int32));
            Columns.Add(RISK_NAMESHORT, typeof(System.String));
            Columns.Add(FATHER, typeof(System.String));
            Columns.Add(STATUS, typeof(System.String));
            Columns.Add(PROBABILITY, typeof(System.Decimal));
            Columns.Add(TOP_RISK, typeof(System.String));
            Columns.Add(DAMAGE, typeof(System.String));
            Columns.Add(COLOR, typeof(System.String));
            Columns.Add(GROUPE_NAME, typeof(System.String));
            Columns.Add(RISK_TREE, typeof(System.String));
            Columns.Add(IS_ROOT, typeof(System.Boolean));
            Columns.Add(VALUE, typeof(System.Decimal));
            Columns.Add(ID_RISK_TREE, typeof(System.Int32));
            Columns.Add(ID_GROUPE, typeof(System.Int32));
            Columns.Add(ID_FATHER, typeof(System.Int32));
            PrimaryKey = PK_COLUMN;
        }       
    }
}
