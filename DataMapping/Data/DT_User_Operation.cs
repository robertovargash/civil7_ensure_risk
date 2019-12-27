using System.Data;

namespace DataMapping.Data
{
    public class DT_User_Operation:DataTable
    {
        public const string USERNAME_COLUMN = "userName";

        public const string ROLE_COLUMN = "Role";

        public const string ID_OPERATION_COLUMN = "idOperation";

        public const string OPERATION_COLUMN = "Operation";

        public const string TABLE_NAME = "UserOperation";

        public DT_User_Operation():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(USERNAME_COLUMN, typeof(System.String));
            PK_COLUMN[1] = this.Columns.Add(ID_OPERATION_COLUMN, typeof(System.Int32));
            this.Columns.Add(ROLE_COLUMN, typeof(System.String));
            this.Columns.Add(OPERATION_COLUMN, typeof(System.String));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_User_Operation dt = new DT_User_Operation();
            return dt;
        }
    }
}
