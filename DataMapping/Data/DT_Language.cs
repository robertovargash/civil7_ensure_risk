using System.Data;

namespace DataMapping.Data
{
    public class DT_Language : DataTable
    {
        public const string TABLE_NAME = "Languaje";

        public const string LANGUAJE_COLUMN = "Languaje";

        public const string CULTURE_COLUMN = "CultureInfo";

        public DT_Language()
        : base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            this.Columns.Add(LANGUAJE_COLUMN, typeof(System.String));
            this.Columns.Add(CULTURE_COLUMN, typeof(System.String));
        }

    }
}
