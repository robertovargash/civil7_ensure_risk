using System.Data;

namespace DataMapping.Data
{
    public class DT_Role_CM : DataTable
    {
        public const string TABLENAME = "Role_CounterM";

        public const string ID_CM = "idCounterM";
        public const string Role = "Role";
        public const string IDROL_COLUMN = "idRol";
        public const string NAME_SHORT = "nameShort";

        public DT_Role_CM():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_CM, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(IDROL_COLUMN, typeof(System.Int32));
            Columns.Add(NAME_SHORT, typeof(System.String));
            Columns.Add(Role, typeof(System.String));
            PrimaryKey = PK_COLUMN;
        }
    }
}
