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

        public DT_WBS_STRUCTURE() : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_FATHER, typeof(System.Int32));
            PK_COLUMN[1] = this.Columns.Add(ID_CHILD, typeof(System.Int32));
            this.Columns.Add(FATHER, typeof(System.String));
            this.Columns.Add(FNIVEL, typeof(System.String));
            this.Columns.Add(CHILD, typeof(System.String));
            this.Columns.Add(CNIVEL, typeof(System.String));
            this.Columns.Add(IDPROJECT, typeof(System.Int32));
            this.PrimaryKey = PK_COLUMN;
        }
    }
}
