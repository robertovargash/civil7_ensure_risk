using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataMapping.Data
{
    public class DT_DefaulRisk:DataTable
    {
        public const string Risk_TABLA = "DefaultRisk";

        public const string ID_COLUMNA = "idDefaultRisk";
        public const string RISK_NAME_COLUMNA = "RiskName";

        public DT_DefaulRisk():base()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[1];
            TableName = Risk_TABLA;
            PK_COLUMN[0] = Columns.Add(ID_COLUMNA, typeof(System.Int32));
            PK_COLUMN[0].AutoIncrement = true; PK_COLUMN[0].AutoIncrementSeed = -100; PK_COLUMN[0].AutoIncrementStep = -1;
            Columns.Add(RISK_NAME_COLUMNA, typeof(System.String));
            PrimaryKey = PK_COLUMN;
        }
    }
}
