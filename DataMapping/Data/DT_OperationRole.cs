using System.Data;

namespace DataMapping.Data
{
    public class DT_OperationRole : DataTable
    {
        public const string OPERATION_ROLE_TABLE = "Role_Operation";

        public const string OPERATION_COLUMN = "Operation";

        public const string ID_OPERATION_COLUM = "idOperation";

        public const string ROLE_COLUM = "Role";
        public const string IDROL_COLUMN = "idRol";


        public DT_OperationRole():base()
        {
            CreateTable();
        }
      

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = OPERATION_ROLE_TABLE;
            PK_COLUMN[0] = this.Columns.Add(ID_OPERATION_COLUM, typeof(int));
            PK_COLUMN[1] = this.Columns.Add(IDROL_COLUMN, typeof(int));
            this.Columns.Add(OPERATION_COLUMN, typeof(string));
            Columns.Add(ROLE_COLUM, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_OperationRole dt = new DT_OperationRole();
            return dt;
        }
    }
}
