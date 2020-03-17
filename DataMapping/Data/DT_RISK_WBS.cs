using System.Data;

namespace DataMapping.Data
{
    public class DT_RISK_WBS : DataTable
    {
        public const string TABLENAME = "Risk_WBS";

        public const string ID_RISK = "idRisk";
        public const string ID_WBS = "idWBS";
        public const string RISK = "Risk";
        public const string WBS = "WBS_Name";
        public const string NIVEL = "Nivel";
        public const string IS_PRIMARY = "isPrimary";
        public const string PRIMARY = "Primaryy";
        public const string USERNAME = "userName";
        public const string PROBABILITY = "probability";

        public DT_RISK_WBS() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_WBS, typeof(System.Int32));
            Columns.Add(RISK, typeof(System.String));
            Columns.Add(WBS, typeof(System.String));
            Columns.Add(NIVEL, typeof(System.String));
            Columns.Add(IS_PRIMARY, typeof(System.Boolean));
            Columns.Add(PRIMARY, typeof(System.String));
            Columns.Add(USERNAME, typeof(System.String));
            Columns.Add(PROBABILITY, typeof(decimal));
            PrimaryKey = PK_COLUMN;
        }
    }
}
