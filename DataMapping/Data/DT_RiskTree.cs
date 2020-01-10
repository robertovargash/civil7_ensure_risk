using System.Data;

namespace DataMapping.Data
{
    public class DT_RiskTree : DataTable
    {
        public const string TABLE_NAME = "RiskTree";

        public const string ID_RISK_TREE = "idRiskTree";
        public const string DIAGRAM_NAME = "RiskTree";
        public const string ROOT_NAME_SHORT = "nameShort";
        public const string ROOT_ID = "idRisk";
        public const string ID_WBS = "idWBS";

        public DT_RiskTree()
            : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK_TREE, typeof(System.Int32));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(DIAGRAM_NAME, typeof(System.String));
            this.Columns.Add(ROOT_NAME_SHORT, typeof(System.String));
            this.Columns.Add(ROOT_ID, typeof(System.Int32));
            this.Columns.Add(ID_WBS, typeof(System.Int32));
            this.PrimaryKey = PK_COLUMN;
        }
      
    }
}
