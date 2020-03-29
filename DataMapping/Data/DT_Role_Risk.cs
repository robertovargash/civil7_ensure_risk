using System.Data;

namespace DataMapping.Data
{
    public class DT_Role_Risk : DataTable
    {
        public const string TABLENAME = "Role_Risk";

        public const string ID_RISK = "idRisk";
        public const string Role = "Role";
        public const string IDROL_COLUMN = "idRol";
        public const string NAME_SHORT = "nameShort";

        public DT_Role_Risk():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            TableName = TABLENAME;
            PK_COLUMN[0] = this.Columns.Add(ID_RISK, typeof(int));
            PK_COLUMN[1] = this.Columns.Add(IDROL_COLUMN, typeof(int));
            Columns.Add(NAME_SHORT, typeof(string));
            Columns.Add(Role, typeof(string));
            PrimaryKey = PK_COLUMN;
        }
    }
}
