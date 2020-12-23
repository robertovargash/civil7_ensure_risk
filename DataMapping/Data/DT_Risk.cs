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
        public const string PROBABILITY = "probability";
        public const string POSITION = "position";
        public const string IS_ACTIVE = "enabled";
        public const string ID_GROUPE = "idGroupe";
        public const string GROUPE_NAME = "GroupeName";
        public const string ID_WBS = "idWBS";
        public const string WBS_NAME = "WBSName";
        public const string USER_NAME = "userName";
        private const string iS_CM = "isCM";
        public static string IS_CM => iS_CM;


        public DT_Risk() : base()
        {
            CreateTable();
        }


        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = TABLE_NAME;
            PK_COLUMN[0] = Columns.Add(ID, typeof(decimal));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(ID_DIAGRAM, typeof(decimal));
            Columns.Add(NAMESHORT, typeof(string));
            Columns.Add(COMMENTS, typeof(string));
            Columns.Add(FATHER, typeof(string));
            Columns.Add(IDRISK_FATHER, typeof(decimal));
            Columns.Add(ISCOLLAPSED, typeof(bool));
            Columns.Add(IS_ROOT, typeof(bool));
            Columns.Add(DIAGONAL, typeof(bool));
            Columns.Add(PROBABILITY, typeof(decimal));
            Columns.Add(POSITION, typeof(int));
            Columns.Add(IS_ACTIVE, typeof(bool));
            Columns.Add(ID_GROUPE, typeof(decimal));
            Columns.Add(GROUPE_NAME, typeof(string));
            Columns.Add(ID_WBS, typeof(decimal));
            Columns.Add(WBS_NAME, typeof(string));
            Columns.Add(USER_NAME, typeof(string));
            Columns.Add(IS_CM, typeof(bool));

            PrimaryKey = PK_COLUMN;
        }
    }
}