using System.Data;

namespace DataMapping.Data
{


    public partial class DT_Damage : DataTable {
		
        public const string TopRisk_TABLA = "TopRisk"; 
        
        public const string COLORID_COLUMNA = "Color";
        public const string ID_COLUMNA = "idTopRisk";
        public const string TOP_RISK_COLUMN = "TopRisk";
        public const string UM = "UM";

        public DT_Damage() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TopRisk_TABLA;
            PK_COLUMN[0] = this.Columns.Add(ID_COLUMNA, typeof(int));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            this.Columns.Add(COLORID_COLUMNA, typeof(string));
            this.Columns.Add(TOP_RISK_COLUMN, typeof(string));
            this.Columns.Add(UM, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_Damage dt = new DT_Damage();
            return dt;
        }

    }
}