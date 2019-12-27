using System.Data.SqlClient;

namespace ERDA
{
    public class SSQLConnection : Connection
    {
        protected string sServer;
        protected string dBase;
        protected string uUser;
        protected string pPassword;

        private const int MAX_POOL_SIZE = 50;
        private const int MIN_POOL_SIZE = 5;
        private const int CONNECTION_TIMEOUT = 240;

        public string Server
        {
            get
            {
                return sServer;
            }
            set
            {
                sServer = value;
            }
        }

        public string DataBase
        {
            get
            {
                return dBase;
            }
            set
            {
                dBase = value;
            }
        }

        public string UUSerrr
        {
            get
            {
                return uUser;
            }
            set
            {
                uUser = value;
            }
        }

        public string Passwordd
        {
            get
            {
                return pPassword;
            }
            set
            {
                pPassword = value;
            }
        }

        public override string CConnectionString
        {
            get
            {

                if (pConnectionString.Length == 0 || this.DataBase.Contains("|"))
                {
                    if (this.Server.Length != 0 & this.DataBase.Length != 0 & this.UUSerrr.Length != 0)
                    {
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                        {
                            MaxPoolSize = MAX_POOL_SIZE,
                            MinPoolSize = MIN_POOL_SIZE,
                            ConnectTimeout = CONNECTION_TIMEOUT,
                            UserID = this.UUSerrr,
                            Password = this.pPassword,
                            DataSource = this.Server,
                            InitialCatalog = this.DataBase
                        };
                        pConnectionString = builder.ToString();
                    }
                }

                return pConnectionString;
            }

            set
            {
                pConnectionString = value;
            }
        }


        public SSQLConnection()
        {

        }


        public override System.Data.IDbConnection Get_Connection()
        {
            return new SqlConnection(this.CConnectionString);
        }

        public override System.Data.IDbTransaction BeginTransaction()
        {
            SqlConnection conn = (SqlConnection)Get_Connection();

            conn.Open();
            return conn.BeginTransaction();
        }

        public override System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel iso)
        {
            SqlConnection conn = (SqlConnection)Get_Connection();

            conn.Open();
            return conn.BeginTransaction(iso);
        }

        public override void EndTransaction(System.Data.IDbTransaction trans)
        {
            trans.Commit();
        }

        public override void AbortTransaction(System.Data.IDbTransaction trans)
        {
            trans.Rollback();
        }

    }

}
