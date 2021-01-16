using System.Data;

namespace DataMapping.Data
{
    public partial class DT_Diagram_Damages:DataTable
    {
        private const string tABLE_NAME = "RiskTree_TopRisk";

        private const string iD_RISKTREE = "idRiskTree";
        private const string iD_DAMAGE = "idTopRisk";
        private const string tOP_RISK = "TopRisk";
        private const string cOLOR = "Color";
        private const string rISK_TREE = "RiskTree";
        private const string uM = "UM";
        private const string dAMAGE = "Damage";

        public static string TABLE_NAME => tABLE_NAME;

        public static string ID_RISKTREE => iD_RISKTREE;

        public static string ID_DAMAGE => iD_DAMAGE;

        public static string TOP_RISK => tOP_RISK;

        public static string COLOR => cOLOR;

        public static string RISK_TREE => rISK_TREE;

        public static string UM => uM;

        public static string DAMAGE => dAMAGE;

        public DT_Diagram_Damages() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISKTREE, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(decimal));
            Columns.Add(RISK_TREE, typeof(string));
            Columns.Add(TOP_RISK, typeof(string));
            Columns.Add(COLOR, typeof(string));
            Columns.Add(UM, typeof(string));
            Columns.Add(DAMAGE, typeof(string));
            PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_Diagram_Damages dt = new DT_Diagram_Damages();
            return dt;
        }
    }
}
