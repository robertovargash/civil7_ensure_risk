using System.Data;

namespace DataMapping.Data
{
    public class DT_CM_WBS: DataTable
    {
        public const string TABLE_NAME = "CM_WBS";

        public const string ID_CM = "idCM";
        public const string ID_WBS = "idWBS";
        public const string CM = "CM";
        public const string WBS = "WBS_Name";
        public const string NIVEL = "Nivel";
        public const string IS_PRIMARY = "isPrimary";
        public const string PRIMARY = "Primaryy";
        public const string USERNAME = "userName";
        public const string PROBABILITY = "probability";

        public DT_CM_WBS() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_CM, typeof(int));
            PK_COLUMN[1] = this.Columns.Add(ID_WBS, typeof(int));
            Columns.Add(CM, typeof(string));
            Columns.Add(WBS, typeof(string));
            Columns.Add(NIVEL, typeof(string));
            Columns.Add(IS_PRIMARY, typeof(bool));
            Columns.Add(PRIMARY, typeof(string));
            Columns.Add(USERNAME, typeof(string));
            Columns.Add(PROBABILITY, typeof(decimal));
            PrimaryKey = PK_COLUMN;
        }
    }
}
