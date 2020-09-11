using System.Data;

namespace DataMapping.Data
{
    public class DT_Role : DataTable
    {

        public const string ROLE_TABLE = "Role";

        public const string ROLE_COLUM = "Role";
        public const string IDROL_COLUMN = "idRol";

        public const string USERNAME_COLUM = "Username";


        public DT_Role()
            : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = ROLE_TABLE;
            PK_COLUMN[0] = this.Columns.Add(IDROL_COLUMN, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ROLE_COLUM, typeof(string));
            this.Columns.Add(USERNAME_COLUM, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_Role dt = new DT_Role();
            return dt;
        }
    }
}
