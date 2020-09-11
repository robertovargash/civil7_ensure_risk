using System.Data;

namespace DataMapping.Data
{
    public class DT_Diagram : DataTable
    {
        public const string TABLE_NAME = "RiskTree";

        public const string ID_DIAGRAM = "idRiskTree";
        public const string DIAGRAM_NAME = "RiskTree";
        public const string ROOT_NAME_SHORT = "nameShort";
        public const string ROOT_ID = "idRisk";
        public const string ID_PROJECT = "idProject";

        public DT_Diagram()
            : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_DIAGRAM, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; 
            PK_COLUMN[0].AutoIncrementSeed = -100; 
            PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(DIAGRAM_NAME, typeof(string));
            this.Columns.Add(ROOT_NAME_SHORT, typeof(string));
            this.Columns.Add(ROOT_ID, typeof(decimal));
            this.Columns.Add(ID_PROJECT, typeof(decimal));
            this.PrimaryKey = PK_COLUMN;
        }
      
    }
}
