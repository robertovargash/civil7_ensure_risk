﻿using System.Data;

namespace DataMapping.Data
{
    public class DT_OperationRole : DataTable
    {
        public const string TABLE_NAME = "Role_Operation";

        public const string OPERATION_COLUMN = "Operation";

        public const string ID_OPERATION_COLUM = "idOperation";

        public const string ROLE_COLUM = "Role";
        public const string IDROL_COLUMN = "idRol";


        public DT_OperationRole():base()
        {
            CreateTable();
        }
      

        private void CreateTable()
        {
            DataColumn[] PK_COLUMN = new DataColumn[2];
            this.TableName = TABLE_NAME;
            PK_COLUMN[0] = this.Columns.Add(ID_OPERATION_COLUM, typeof(decimal));
            PK_COLUMN[1] = this.Columns.Add(IDROL_COLUMN, typeof(decimal));
            this.Columns.Add(OPERATION_COLUMN, typeof(string));
            Columns.Add(ROLE_COLUM, typeof(string));
            this.PrimaryKey = PK_COLUMN;
        }
        public static DataTable Create()
        {
            DT_OperationRole dt = new DT_OperationRole();
            return dt;
        }
    }
}
