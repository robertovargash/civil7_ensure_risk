using System.Data;

namespace DataMapping.Data
{
    public class DT_Diagram_Damages:DataTable
    {
        public const string TABLE_NAME = "RiskTree_TopRisk";

        public const string ID_RISKTREE = "idRiskTree";
        public const string ID_DAMAGE = "idTopRisk";
        public const string TOP_RISK = "TopRisk";
        public const string COLOR= "Color";
        public const string RISK_TREE = "RiskTree"; 
        public const string UM = "UM";
        public const string DAMAGE = "Damage";

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
