using System;
using System.Data;


namespace ERDA
{
   
    public abstract class Connection
    {
        protected string pConnectionString;

        public abstract string CConnectionString { get; set; }

        public abstract IDbConnection Get_Connection();

        public abstract IDbTransaction BeginTransaction();

        public abstract IDbTransaction BeginTransaction(System.Data.IsolationLevel iso);

        public abstract void EndTransaction(IDbTransaction trans);

        public abstract void AbortTransaction(IDbTransaction trans);

    }
}
