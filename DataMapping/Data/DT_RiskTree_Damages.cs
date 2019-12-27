using System.Data;

namespace DataMapping.Data
{
    public class DT_RiskTree_Damages:DataTable
    {
        public const string TABLENAME = "RiskTree_TopRisk";

        public const string ID_RISKTREE = "idRiskTree";
        public const string ID_DAMAGE = "idTopRisk";
        public const string DAMAGE = "TopRisk";
        public const string COLOR= "Color";
        public const string RISK_TREE = "RiskTree";
        public const string UM = "UM";

        public DT_RiskTree_Damages():base()
        {
            CreateTable();
        }

        private void CreateTable()  
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISKTREE, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(System.Int32));
            Columns.Add(RISK_TREE, typeof(System.String));
            Columns.Add(DAMAGE, typeof(System.String));
            Columns.Add(COLOR, typeof(System.String));
            Columns.Add(UM, typeof(System.String));
            PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_RiskTree_Damages dt = new DT_RiskTree_Damages();
            return dt;
        }
    }
}
