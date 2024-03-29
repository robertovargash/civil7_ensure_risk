using System.Data;

namespace DataMapping.Data
{


    public partial class DT_User : DataTable {
		
        public const string TABLE_NAME = "Username"; 

        public const string USERNAME = "userName";
        public const string USERPASSWORD= "userPassword";
        public const string POSITIONDESCRIPTION = "positionDescription";
        public const string FULL_NAME = "fullName";

        public DT_User() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(USERNAME, typeof(string));
            //PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(USERPASSWORD, typeof(System.Byte[]));
            this.Columns.Add(POSITIONDESCRIPTION, typeof(string));
            this.Columns.Add(FULL_NAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_User dt = new DT_User();
            return dt;
        }

    }
}