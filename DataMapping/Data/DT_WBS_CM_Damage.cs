using System.Data;

namespace DataMapping.Data
{
    public class DT_WBS_CM_Damage : DataTable
    {
        public const string ID_WBS = "idWBS";
        private const string wBS = "WBSName";
        public const string ID_CM = "idCM";
        public const string ID_DAMAGE = "idTopRisk";
        public const string DAMAGE = "Damage";
        public const string VALUE = "value";
        public const string TABLE_NAME = "CM_WBS_Damage"; 
        private const string wBS_USER = "WBS_User";

        public static string WBS_USER => wBS_USER;

        public static string WBS => wBS;

        public DT_WBS_CM_Damage() : base()
        {
                CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[3];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_WBS, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(ID_DAMAGE, typeof(decimal));
            PK_COLUMN[2] = this.Columns.Add(ID_CM, typeof(decimal));
            Columns.Add(WBS, typeof(string));
            Columns.Add(DAMAGE, typeof(string));
            Columns.Add(WBS_USER, typeof(string));
            Columns.Add(VALUE, typeof(decimal));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
