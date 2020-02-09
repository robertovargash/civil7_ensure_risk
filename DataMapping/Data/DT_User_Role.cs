using System.Data;

namespace DataMapping.Data
{
    public class DT_User_Role: DataTable
    {
        public const string USER = "userName";

        public const string ROLE = "Role";

        public const string TABLE_NAME = "UserName_Role";
        public const string IDROL_COLUMN = "idRol";

        public DT_User_Role():base()
        {
            CreateTable();
        }
        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(USER, typeof(System.String));
            PK_COLUMN[1] = this.Columns.Add(IDROL_COLUMN, typeof(System.Int32));
            Columns.Add(ROLE, typeof(System.String));
            this.PrimaryKey = PK_COLUMN;
        }
      
    }
}
