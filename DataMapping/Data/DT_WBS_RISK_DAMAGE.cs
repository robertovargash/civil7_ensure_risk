using System.Data;

namespace DataMapping.Data
{
    public class DT_WBS_RISK_DAMAGE:DataTable
    {
        public const string ID_WBS = "idWBS";
        public const string WBS = "WBSName";
        public const string ID_RISK = "idRisk";
        public const string ID_DAMAGE = "idTopRisk";
        public const string DAMAGE = "Damage";
        public const string VALUE = "value";
        public const string TABLE_NAME = "Risk_WBS_Damage";


        public DT_WBS_RISK_DAMAGE():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[3];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_WBS, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(System.Int32));
            PK_COLUMN[2] = this.Columns.Add(ID_RISK, typeof(System.Int32));
            Columns.Add(WBS, typeof(System.String));
            Columns.Add(DAMAGE, typeof(System.String));
            Columns.Add(VALUE, typeof(System.Decimal));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
