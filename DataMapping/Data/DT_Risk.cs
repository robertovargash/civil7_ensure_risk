using System;
using System.Data;
using System.Linq;

namespace DataMapping.Data
{
    public partial class DT_Risk : DataTable {
		
        public const string TABLE_NAME = "Risk"; 

        public const string ID_COLUMNA = "idRisk";
        public const string ID_RISK_TREE = "idRiskTree";
        public const string NAMESHORT_COLUMNA = "nameShort";
        public const string DETAIL_COLUMNA = "detail";
        public const string FATHER_COLUMNA = "Father";
        public const string IDRISK_FATHER = "idRiskFather";
        public const string ISCOLLAPSEDINGUI_COLUMNA = "isCollapsedInGUI";
        public const string IS_ROOT_COLUMNA = "isRoot";
        public const string DIAGONAL_COLUMNA = "diagonal";
        public const string FROM_TOP_COLUMNA = "fromTop";
        public const string PROBABILITY_COLUMN = "probability";
        public const string POSITION_COLUMN = "position";
        public const string ENABLED_COLUMN = "enabled";
        public const string ID_GROUPE_COLUMN = "idGroupe";
        public const string GROUPE_NAME_COLUMN = "GroupeName";

        public DT_Risk() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID_COLUMNA, typeof(System.Int32));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ID_RISK_TREE, typeof(System.Int32));
            Columns.Add(NAMESHORT_COLUMNA, typeof(System.String));
            Columns.Add(DETAIL_COLUMNA, typeof(System.String));
            Columns.Add(FATHER_COLUMNA, typeof(System.String));
            Columns.Add(IDRISK_FATHER, typeof(System.Int32));
            Columns.Add(ISCOLLAPSEDINGUI_COLUMNA, typeof(System.Boolean));
            Columns.Add(IS_ROOT_COLUMNA, typeof(System.Boolean));
            Columns.Add(DIAGONAL_COLUMNA, typeof(System.Boolean));
            Columns.Add(FROM_TOP_COLUMNA, typeof(System.Boolean));
            Columns.Add(PROBABILITY_COLUMN, typeof(System.Decimal));
            Columns.Add(POSITION_COLUMN, typeof(System.Int32));
            Columns.Add(ENABLED_COLUMN, typeof(System.Boolean));
            Columns.Add(ID_GROUPE_COLUMN, typeof(System.Int32));
            Columns.Add(GROUPE_NAME_COLUMN, typeof(System.String));

            PrimaryKey = PK_COLUMN;
        }
    }
}