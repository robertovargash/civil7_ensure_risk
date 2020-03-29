using System.Data;

namespace DataMapping.Data
{
    public class DT_WBS_STRUCTURE:DataTable
    {
        public const string TABLE_NAME = "WBSStructure";

        public const string ID_FATHER = "idFather";
        public const string ID_CHILD = "idChild";
        public const string FATHER = "Father";
        public const string FNIVEL = "FatherNivel";
        public const string CHILD = "Child";        
        public const string CNIVEL = "ChildNivel";
        public const string IDPROJECT = "idProject";
        public const string IS_MANAGER = "isManager";
        public const string CHILD_USER = "CUserName";
        public const string FATHER_USER = "FUserName";
        public DT_WBS_STRUCTURE() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_FATHER, typeof(int));
            PK_COLUMN[1] = this.Columns.Add(ID_CHILD, typeof(int));
            this.Columns.Add(FATHER, typeof(string));
            this.Columns.Add(FNIVEL, typeof(string));
            this.Columns.Add(CHILD, typeof(string));
            this.Columns.Add(CNIVEL, typeof(string));
            this.Columns.Add(IDPROJECT, typeof(int));
            this.Columns.Add(IS_MANAGER, typeof(bool));
            this.Columns.Add(CHILD_USER, typeof(string));
            this.Columns.Add(FATHER_USER, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
