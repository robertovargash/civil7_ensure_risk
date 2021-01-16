
using System.Data;

namespace DataMapping.Data
{
    public partial class DT_Project : DataTable
    {
        private const string tABLE_NAME = "Project";

        private const string iD_PROJECT = "idProject";
        private const string pROJECT_NAME = "ProjectName";

        public static string TABLE_NAME => tABLE_NAME;

        public static string ID_PROJECT => iD_PROJECT;

        public static string PROJECT_NAME => pROJECT_NAME;

        public DT_Project() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_PROJECT, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(PROJECT_NAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
