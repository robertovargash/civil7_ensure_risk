using System.Data;

namespace DataMapping.Data
{
    public partial class DT_Diagram : DataTable
    {
        private const string tABLE_NAME = "RiskTree";

        private const string iD_DIAGRAM = "idRiskTree";
        private const string dIAGRAM_NAME = "RiskTree";
        private const string rOOT_NAME_SHORT = "nameShort";
        private const string rOOT_ID = "idRisk";
        private const string iD_PROJECT = "idProject";

        public static string TABLE_NAME => tABLE_NAME;

        public static string ID_DIAGRAM => iD_DIAGRAM;

        public static string DIAGRAM_NAME => dIAGRAM_NAME;

        public static string ROOT_NAME_SHORT => rOOT_NAME_SHORT;

        public static string ROOT_ID => rOOT_ID;

        public static string ID_PROJECT => iD_PROJECT;

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
