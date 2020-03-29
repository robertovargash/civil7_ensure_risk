
using System.Data;

namespace DataMapping.Data
{
    public class DT_Project : DataTable
    {
        public const string TABLE_NAME = "Project";

        public const string ID_PROJECT = "idProject";
        public const string PROJECT_NAME = "ProjectName";

        public DT_Project() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_PROJECT, typeof(int));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(PROJECT_NAME, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
