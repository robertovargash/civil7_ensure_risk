using System.Data;

namespace DataMapping.Data
{


    public partial class DT_RiskStructure : DataTable {
		
        public const string TABLE_NAME = "RiskStructure"; 

        //public const string ID_COLUMNA = "id";
        public const string IDRISK = "idRisk";
        public const string IDRISK_FATHER = "idRiskFather";
        public const string POSITION_COLUMN = "position";

        public DT_RiskStructure() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(IDRISK, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(IDRISK_FATHER, typeof(System.Int32));
            //PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            //this.Columns.Add(IDRISK_COLUMNA, typeof(System.Int32));
            Columns.Add(POSITION_COLUMN, typeof(System.Int32));
            PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_RiskStructure dt = new DT_RiskStructure();
            return dt;
        }

    }
}