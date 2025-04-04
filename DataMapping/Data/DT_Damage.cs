using System.Data;

namespace DataMapping.Data
{
    public partial class DT_Damage : DataTable {

        private const string tABLE_NAME = "TopRisk";
        private const string cOLORID_COLUMNA = "Color";
        private const string iD_COLUMNA = "idTopRisk";
        private const string tOP_RISK_COLUMN = "TopRisk";
        private const string uM = "UM";

        public static string COLORID_COLUMNA => cOLORID_COLUMNA;

        public static string ID_COLUMNA => iD_COLUMNA;

        public static string TOP_RISK_COLUMN => tOP_RISK_COLUMN;

        public static string UM => uM;

        public static string TABLE_NAME => tABLE_NAME;

        public DT_Damage() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_COLUMNA, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; 
            PK_COLUMN[0].AutoIncrementSeed = -100; 
            PK_COLUMN[0].AutoIncrementStep = -1;
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