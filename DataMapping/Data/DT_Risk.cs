using System;
using System.Data;
using System.Linq;

namespace DataMapping.Data
{
    public partial class DT_Risk : DataTable {
		
        public const string TABLE_NAME = "Risk"; 

        public const string ID = "idRisk";
        public const string ID_DIAGRAM = "idRiskTree";
        public const string NAMESHORT = "nameShort";
        public const string COMMENTS = "detail";
        public const string FATHER = "Father";
        public const string IDRISK_FATHER = "idRiskFather";
        public const string ISCOLLAPSED = "isCollapsedInGUI";
        public const string IS_ROOT = "isRoot";
        public const string DIAGONAL = "diagonal";
        public const string FROM_TOP = "fromTop";
        public const string PROBABILITY = "probability";
        public const string POSITION = "position";
        public const string ENABLED = "enabled";
        public const string ID_GROUPE = "idGroupe";
        public const string GROUPE_NAME = "GroupeName";
        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string USER_NAME = "userName";


        public DT_Risk() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID, typeof(System.Int32));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ID_DIAGRAM, typeof(System.Int32));
            Columns.Add(NAMESHORT, typeof(System.String));
            Columns.Add(COMMENTS, typeof(System.String));
            Columns.Add(FATHER, typeof(System.String));
            Columns.Add(IDRISK_FATHER, typeof(System.Int32));
            Columns.Add(ISCOLLAPSED, typeof(System.Boolean));
            Columns.Add(IS_ROOT, typeof(System.Boolean));
            Columns.Add(DIAGONAL, typeof(System.Boolean));
            Columns.Add(FROM_TOP, typeof(System.Boolean));
            Columns.Add(PROBABILITY, typeof(System.Decimal));
            Columns.Add(POSITION, typeof(System.Int32));
            Columns.Add(ENABLED, typeof(System.Boolean));
            Columns.Add(ID_GROUPE, typeof(System.Int32));
            Columns.Add(GROUPE_NAME, typeof(System.String));
            Columns.Add(ID_WBS, typeof(System.Int32));
            Columns.Add(WBS_NAME, typeof(System.String));
            Columns.Add(USER_NAME, typeof(System.String));

            PrimaryKey = PK_COLUMN;
        }
    }
}