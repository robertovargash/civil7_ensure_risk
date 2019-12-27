using System.Data.SqlClient;
using System.Reflection;
using System.Configuration;
using System.Data;
using System;

namespace ERDA
{
    public class SQLAccessBuilder : ERDA
    {
        #region Variables

        private readonly string strTableName; 
        private readonly DataColumn[] pkColumns;

        private DataTable dtMain;

        private const char TIPOACTUALIZACION_INSERT = 'I';
        private const char TIPOACTUALIZACION_UPDATE = 'U';
        private const char TIPOACTUALIZACION_DELETE = 'D';

        protected string pa_LOAD;
        protected string pa_INSERT;
        protected string pa_UPDATE;
        protected string pa_DELETE;

        protected int IDUserConnected;

        private const string USER = "ERDA.My.MySettings.USER";
        #endregion

        #region PROPERTIES
       
        public int UsuarioConectado
        {
            get
            {
                return IDUserConnected;
            }
            set
            {
                IDUserConnected = value;
            }
        }

        #endregion

        #region MetodosPrivados

        private SqlDbType GetDataType(string typeName)
    {
        switch (typeName.ToUpper())
        {
            case "BIGINT":
                return System.Data.SqlDbType.BigInt;
            case "BINARY":
                return System.Data.SqlDbType.Binary;
            case "IMAGE":
                return System.Data.SqlDbType.Image;
            case "VARBINARY":
                return System.Data.SqlDbType.VarBinary;
            case "BIT":
                return System.Data.SqlDbType.Bit;
            case "CHAR":
                return System.Data.SqlDbType.Char;
            case "NCHAR":
                return System.Data.SqlDbType.NChar;
            case "NTEXT":
                return System.Data.SqlDbType.NText;
            case "NVARCHAR":
                return System.Data.SqlDbType.NVarChar;
            case "TEXT":
                return System.Data.SqlDbType.Text;
            case "VARCHAR":
                return System.Data.SqlDbType.VarChar;
            case "DATETIME":
                return System.Data.SqlDbType.DateTime;
            case "DATE":
                return System.Data.SqlDbType.Date;
            case "SMALLDATETIME":
                return System.Data.SqlDbType.SmallDateTime;
            case "TIMESTAMP":
                return System.Data.SqlDbType.Timestamp;
            case "DECIMAL":
                return System.Data.SqlDbType.Decimal;
            case "NUMERIC":
                return System.Data.SqlDbType.Decimal;
            case "MONEY":
                return System.Data.SqlDbType.Money;
            case "SMALLMONEY":
                return System.Data.SqlDbType.SmallMoney;
            case "FLOAT":
                return System.Data.SqlDbType.Float;
            case "INT":
                return System.Data.SqlDbType.Int;
            case "REAL":
                return System.Data.SqlDbType.Real;
            case "SMALLINT":
                return System.Data.SqlDbType.SmallInt;
            case "TINYINT":
                return System.Data.SqlDbType.TinyInt;
            case "UNIQUEIDENTIFIER":
                return System.Data.SqlDbType.UniqueIdentifier;
            case "VARIANT":
                return System.Data.SqlDbType.Variant;
            case "DATATABLE":
                return System.Data.SqlDbType.Structured;
            case "XML":
                return System.Data.SqlDbType.Xml;
            default:
                DataTable dt = this.ObtenerTipo(typeName);
                return GetDataType(Convert.ToString(dt.Rows[0]["Storage_type"]));
        }
    }

        private System.Data.SqlClient.SqlCommand GetInsertCommand()
    {
        if (insertCommand == null)
        {
            insertCommand = new System.Data.SqlClient.SqlCommand();
            this.insertCommand.CommandText = pa_INSERT;
            this.insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
            if (this.adTransaction != null)
            {
                insertCommand.Transaction = this.adTransaction;
                insertCommand.Connection = this.adTransaction.Connection;
            }
            else
            {
                this.insertCommand.Connection = (System.Data.Common.DbConnection)adConnection.Get_Connection();
                }
            this.insertCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, (byte)0, (byte)0, "", System.Data.DataRowVersion.Current, null));
            foreach (DataRow dr in dtMain.Rows)
            {

                if (!(pkColumns[0].AutoIncrement && pkColumns[0].ColumnName == dr["COLUMN_NAME"].ToString()))
                {
                    if (!(Convert.ToString(dr["TYPE_NAME"]).ToUpper().EndsWith("IDENTITY")))
                    {
                        SqlDbType SQLDataType = GetDataType(Convert.ToString(dr["TYPE_NAME"]));
                        if (!(SQLDataType == SqlDbType.UniqueIdentifier & dr["COLUMN_NAME"].ToString().ToUpper().StartsWith("ROWGUID")))
                        {
                            int precision = 0;
                            if (Convert.ToString(dr["TYPE_NAME"]) == SQLDataType.ToString())
                            {
                                precision = Convert.ToInt32(dr["LENGTH"]);
                            }
                            else
                            {
                                precision = Convert.ToInt32(dr["PRECISION"]);
                            }
                            this.insertCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@" + Convert.ToString(dr["COLUMN_NAME"]), SQLDataType, precision, Convert.ToString(dr["COLUMN_NAME"])));
                        }
                    }
                }
                //If Not (pkColumns(0).AutoIncrement And pkColumns(0).ColumnName = dr("COLUMN_NAME")) And Not GetDataType(dr("TYPE_NAME")) = SqlDbType.UniqueIdentifier Then
                //    Me.insertCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@" & dr("COLUMN_NAME"), GetDataType(dr("TYPE_NAME")), dr("LENGTH"), dr("COLUMN_NAME")))
                //End If
            }
        }
        return (SqlCommand)insertCommand;
    }

        private SqlCommand GetUpdatecommand()
    {
        if (updateCommand == null)
        {
            updateCommand = new System.Data.SqlClient.SqlCommand();
            this.updateCommand.CommandText = pa_UPDATE;
            this.updateCommand.CommandType = System.Data.CommandType.StoredProcedure;
            if (this.adTransaction != null)
            {
                updateCommand.Transaction = this.adTransaction;
                updateCommand.Connection = this.adTransaction.Connection;
            }
            else
            {
                this.updateCommand.Connection = (System.Data.Common.DbConnection)adConnection.Get_Connection();
            }
            this.updateCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, (byte)0, (byte)0, "", System.Data.DataRowVersion.Current, null));
            //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#:
            //			DataRow dr = null;
            foreach (DataRow dr in dtMain.Rows)
            {
                if (!(Convert.ToString(dr["TYPE_NAME"]).ToUpper().EndsWith("IDENTITY")))
                {
                    SqlDbType SQLDataType = GetDataType(Convert.ToString(dr["TYPE_NAME"]));
                    if (!(SQLDataType == SqlDbType.UniqueIdentifier & dr["COLUMN_NAME"].ToString().ToUpper().StartsWith("ROWGUID"))) //If Not SQLDataType = SqlDbType.UniqueIdentifier Then
                    {
                        int precision = 0;
                        if (Convert.ToString(dr["TYPE_NAME"]) == SQLDataType.ToString())
                        {
                            precision = Convert.ToInt32(dr["LENGTH"]);
                        }
                        else
                        {
                            precision = Convert.ToInt32(dr["PRECISION"]);
                        }
                        this.updateCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@" + Convert.ToString(dr["COLUMN_NAME"]), SQLDataType, precision, Convert.ToString(dr["COLUMN_NAME"])));
                    }
                }
                //If Not GetDataType(dr("TYPE_NAME")) = SqlDbType.UniqueIdentifier Then
                //    Me.updateCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@" & dr("COLUMN_NAME"), GetDataType(dr("TYPE_NAME")), dr("LENGTH"), dr("COLUMN_NAME")))
                //End If
            }
            foreach (DataRow dr in dtMain.Rows) //Original Values
            {
                if (!(Convert.ToString(dr["TYPE_NAME"]).ToUpper().EndsWith("IDENTITY")))
                {
                    SqlDbType SQLDataType = GetDataType(Convert.ToString(dr["TYPE_NAME"]));
                    if (!(SQLDataType == SqlDbType.UniqueIdentifier & dr["COLUMN_NAME"].ToString().ToUpper().StartsWith("ROWGUID"))) //If Not SQLDataType = SqlDbType.UniqueIdentifier Then
                    {
                        //Not (pkColumns(0).AutoIncrement And pkColumns(0).ColumnName = CType(dr("COLUMN_NAME"), System.String)) And 
                        int precision = 0;
                        if (Convert.ToString(dr["TYPE_NAME"]) == SQLDataType.ToString())
                        {
                            precision = Convert.ToInt32(dr["LENGTH"]);
                        }
                        else
                        {
                            precision = Convert.ToInt32(dr["PRECISION"]);
                        }
                        this.updateCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Original_" + Convert.ToString(dr["COLUMN_NAME"]), SQLDataType, precision, System.Data.ParameterDirection.Input, false, (byte)0, (byte)0, Convert.ToString(dr["COLUMN_NAME"]), System.Data.DataRowVersion.Original, null));
                    }
                }
                //If Not GetDataType(dr("TYPE_NAME")) = SqlDbType.UniqueIdentifier Then
                //    Me.updateCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_" & dr("COLUMN_NAME"), GetDataType(dr("TYPE_NAME")), dr("LENGTH"), System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), dr("COLUMN_NAME"), System.Data.DataRowVersion.Original, Nothing))
                //End If
            }
        }
        return (SqlCommand)updateCommand;
    }

        private System.Data.SqlClient.SqlCommand GetDeleteCommand()
    {
        if (deleteCommand == null)
        {
            deleteCommand = new System.Data.SqlClient.SqlCommand();
            this.deleteCommand.CommandText = pa_DELETE;
            this.deleteCommand.CommandType = System.Data.CommandType.StoredProcedure;
            if (this.adTransaction != null)
            {
                deleteCommand.Transaction = this.adTransaction;
                deleteCommand.Connection = this.adTransaction.Connection;
            }
            else
            {
                this.deleteCommand.Connection = (System.Data.Common.DbConnection)adConnection.Get_Connection();
                }
            this.deleteCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, (byte)0, (byte)0, "", System.Data.DataRowVersion.Current, null));
            //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#:
            //			DataRow dr = null;
            foreach (DataRow dr in dtMain.Rows) //Original Values
            {
                if (!(Convert.ToString(dr["TYPE_NAME"]).ToUpper().EndsWith("IDENTITY")))
                {
                    SqlDbType SQLDataType = GetDataType(Convert.ToString(dr["TYPE_NAME"]));
                    if (!(SQLDataType == SqlDbType.UniqueIdentifier & dr["COLUMN_NAME"].ToString().ToUpper().StartsWith("ROWGUID"))) //If Not SQLDataType = SqlDbType.UniqueIdentifier Then
                    {
                        //Not (pkColumns(0).AutoIncrement And pkColumns(0).ColumnName = CType(dr("COLUMN_NAME"), System.String)) And 
                        int precision = 0;
                        if (Convert.ToString(dr["TYPE_NAME"]) == SQLDataType.ToString())
                        {
                            precision = Convert.ToInt32(dr["LENGTH"]);
                        }
                        else
                        {
                            precision = Convert.ToInt32(dr["PRECISION"]);
                        }
                        this.deleteCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Original_" + Convert.ToString(dr["COLUMN_NAME"]), SQLDataType, precision, System.Data.ParameterDirection.Input, false, (byte)0, (byte)0, Convert.ToString(dr["COLUMN_NAME"]), System.Data.DataRowVersion.Original, null));
                    }
                }
                else
                {
                    string strType = Convert.ToString(dr["TYPE_NAME"]).ToUpper();
                    strType = strType.Replace("IDENTITY", string.Empty);
                    strType = strType.Trim();

                    SqlDbType SQLDataType = GetDataType(strType);
                    if (!(SQLDataType == SqlDbType.UniqueIdentifier & dr["COLUMN_NAME"].ToString().ToUpper().StartsWith("ROWGUID"))) // If Not SQLDataType = SqlDbType.UniqueIdentifier Then
                    {
                        //Not (pkColumns(0).AutoIncrement And pkColumns(0).ColumnName = CType(dr("COLUMN_NAME"), System.String)) And 
                        int precision = 0;
                        if (strType == SQLDataType.ToString().ToUpper())
                        {
                            precision = Convert.ToInt32(dr["LENGTH"]);
                        }
                        else
                        {
                            precision = Convert.ToInt32(dr["PRECISION"]);
                        }
                        this.deleteCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Original_" + Convert.ToString(dr["COLUMN_NAME"]), SQLDataType, precision, System.Data.ParameterDirection.Input, false, (byte)0, (byte)0, Convert.ToString(dr["COLUMN_NAME"]), System.Data.DataRowVersion.Original, null));
                    }
                }
                //If Not GetDataType(dr("TYPE_NAME")) = SqlDbType.UniqueIdentifier Then
                //    Me.deleteCommand.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_" & dr("COLUMN_NAME"), GetDataType(dr("TYPE_NAME")), dr("LENGTH"), System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), dr("COLUMN_NAME"), System.Data.DataRowVersion.Original, Nothing))
                //End If
            }
        }
        return (SqlCommand)deleteCommand;
    }

        private void Mapping()
        {
            adDataAdapter.TableMappings.Add("Table", strTableName);
            pa_LOAD = "[pa_Select" + strTableName + "]";
            pa_INSERT = "[pa_Insert" + strTableName + "]";
            pa_UPDATE = "[pa_Update" + strTableName + "]";
            pa_DELETE = "[pa_Delete" + strTableName + "]";
            ((SqlDataAdapter)adDataAdapter).RowUpdated += Tabla_RowUpdated;
        }

        private void Tabla_RowUpdated(object sender, SqlRowUpdatedEventArgs e)
        {
            if (e.Status == UpdateStatus.ErrorsOccurred)
            {
                //If e.Errors.Message.Contains("Violation of UNIQUE KEY constraint") Then
                //    e.Row.RowError = "El documento ya existe"
                //Else
                e.Row.RowError = e.Errors.Message;
                //End If
            }
            else
            {
                if (e.StatementType == StatementType.Insert)
                {
                    e.Status = UpdateStatus.SkipCurrentRow;
                }
                if (e.StatementType == StatementType.Delete)
                {
                    e.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }

        private DataTable ObtenerEstructura(string NombreTabla)
        {
            DataTable tempObtenerEstructura = null;
            const string sp = "sp_columns";
            //Dim Command As New SqlCommand()
            SqlDataAdapter Adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();

            SqlDataAdapter tempVar = (SqlDataAdapter)adDataAdapter;
            try
            {
                tempVar.SelectCommand = GetLoadCommand(sp);
                if (tempVar.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    tempVar.SelectCommand.Connection.Open();
                }

                tempVar.SelectCommand.Parameters.Add("@table_name", SqlDbType.NVarChar, 384).Value = NombreTabla;
                tempVar.Fill(dt);

            }
            finally
            {
                if (tempVar.SelectCommand != null)
                {
                    tempVar.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Connection.Close();
                    }
                }
                tempVar.Dispose();
                tempObtenerEstructura = dt;
            }
            return tempObtenerEstructura;
        }

        private DataTable ObtenerTipo(string NombreTipo)
        {
            DataTable tempObtenerTipo = null;
            const string sp = "sp_help";
            SqlDataAdapter Adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();

            SqlDataAdapter tempVar = (SqlDataAdapter)adDataAdapter;
            try
            {
                tempVar.SelectCommand = GetLoadCommand(sp);
                if (tempVar.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    tempVar.SelectCommand.Connection.Open();
                }

                tempVar.SelectCommand.Parameters.Add("@objname ", SqlDbType.NVarChar, 776).Value = NombreTipo;
                tempVar.Fill(dt);
            }
            finally
            {
                if (tempVar.SelectCommand != null)
                {
                    tempVar.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Connection.Close();
                    }
                }
                tempVar.Dispose();
                tempObtenerTipo = dt;
            }
            return tempObtenerTipo;
        }

        protected void GetParameters(ref SqlCommand SqlCommandToFillParameters, string procedimiento)
        {
            const string sp = "PA_GET_SP_Parameters";
            SqlDataAdapter Adapter = new SqlDataAdapter();
            DataTable dt = new DataTable();
            //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#:
            //		DataRow dr = null;
            //DataRow[] drs = null;

            try
            {
                Adapter.SelectCommand = GetLoadCommand(sp);
                if (Adapter.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    Adapter.SelectCommand.Connection.Open();
                }

                Adapter.SelectCommand.Parameters.Add("@spname", SqlDbType.NVarChar, 100).Value = procedimiento;
                Adapter.Fill(dt);
                //DataRow[] drs = dt.Select("", "ORDINAL_POSITION ASC");
                foreach (DataRow dr in dt.Select("", "ORDINAL_POSITION ASC"))
                {
                    object value = "PARAMETER_NAME";
                    SqlCommandToFillParameters.Parameters.AddWithValue(dr["PARAMETER_NAME"].ToString(), dr["DATA_TYPE"]);
                }
            }
            finally
            {
                if (Adapter.SelectCommand != null)
                {
                    Adapter.SelectCommand.Dispose();
                }
                Adapter.Dispose();
            }

        }

        protected DataRow[] ObtenerFilasEliminadas(DataTable tabla)
        {
            DataRow[] Filas = null;

            if (tabla == null)
            {
                return Filas;
            }

            Filas = tabla.Select("", "", DataViewRowState.Deleted);
            if (Filas.Length == 0 || Filas[0] != null)
            {
                return Filas;
            }

            //INSTANT C# NOTE: Commented this declaration since looping variables in 'foreach' loops are declared in the 'foreach' header in C#:
            //		DataRow f = null;
            int i = 0;
            foreach (DataRow f in tabla.Rows)
            {
                if (f.RowState == DataRowState.Deleted)
                {
                    Filas[i] = f;
                    i += 1;
                }
            }
            return Filas;
        }

        protected SqlCommand GetLoadCommand(string procedimiento)
        {
            if (adTransaction != null)
            {
                loadCommand = new SqlCommand(procedimiento, (SqlConnection)adTransaction.Connection) { Transaction = (SqlTransaction)adTransaction };
            }
            else
            {
                SqlConnection NEWSQL = (SqlConnection)adConnection.Get_Connection();
                loadCommand = new SqlCommand(procedimiento, NEWSQL);
            }
            loadCommand.CommandType = CommandType.StoredProcedure;
            return (SqlCommand)loadCommand;
        }

        //protected SqlCommand GetLoadCommand(string procedimiento, int Timeout)
        //{
        //    if (adTransaction != null)
        //    {
        //        loadCommand = new SqlCommand(procedimiento, (SqlConnection)adTransaction.Connection) { Transaction = (SqlTransaction)adTransaction };
        //    }
        //    else
        //    {
        //        loadCommand = new SqlCommand(procedimiento, (SqlConnection)adConnection.Get_Connection());
        //    }
        //    loadCommand.CommandType = CommandType.StoredProcedure;
        //    loadCommand.CommandTimeout = 180;
        //    return (SqlCommand)loadCommand;
        //}

        #endregion

        #region COnstructoresDestructores

        private void SetServidor()
        {

            SqlConnectionStringBuilder builder = AppConectionString(USER);
            ((SSQLConnection)adConnection).Server = builder.DataSource;
            ((SSQLConnection)adConnection).CConnectionString = builder.ConnectionString;
        }

        private void ConfigdsConection()
        {
            SqlConnectionStringBuilder builderSQLServer = AppConectionString(USER);
            ((SSQLConnection)adConnection).DataBase = builderSQLServer.InitialCatalog;
            ((SSQLConnection)adConnection).UUSerrr = builderSQLServer.UserID;
            ((SSQLConnection)adConnection).Passwordd = builderSQLServer.Password;
            SetServidor();
        }

        private void ConfigdsConection(int TiempoEsperaConexion)
        {
            ConfigdsConection();

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(adConnection.CConnectionString) { ConnectTimeout = TiempoEsperaConexion };
            adConnection.CConnectionString = builder.ConnectionString;
        }

        private void ConfigdsConection(string Base)
        {
            ((SSQLConnection)adConnection).DataBase = Base;

            SqlConnectionStringBuilder builder = AppConectionString(USER);

            ((SSQLConnection)adConnection).UUSerrr = builder.UserID;
            ((SSQLConnection)adConnection).Passwordd = builder.Password;
            SetServidor();
        }

        private void ConfigdsConection(string Base, int TiempoEsperaConexion)
        {
            ConfigdsConection(Base);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(adConnection.CConnectionString);
            builder.ConnectTimeout = TiempoEsperaConexion;
            adConnection.CConnectionString = builder.ConnectionString;
        }

        private static SqlConnectionStringBuilder AppConectionString(string NombreConexion)
        {
            try
            {
                Assembly MyAssembly = Assembly.GetExecutingAssembly();
                string AplicationPath = MyAssembly.CodeBase.Substring(8, MyAssembly.CodeBase.LastIndexOf('/') - 7);

                Configuration config = HandleConfiguration.DesEncriptarConfiguracion(AplicationPath + "ERDA.dll"); //Des encripta la configuración


                ConnectionStringsSection connections = config.ConnectionStrings;

                SqlConnectionStringBuilder sqlConStrBuilder = new SqlConnectionStringBuilder { ConnectionString = connections.ConnectionStrings[NombreConexion].ConnectionString };
                return sqlConStrBuilder;
            }
            catch (Exception)
            {
                return null;
            }            
        }

        private static SqlConnectionStringBuilder AppConectionStringMySQL(string NombreConexion)
        {
            try
            {
                Assembly MyAssembly = Assembly.GetExecutingAssembly();
                string AplicationPath = MyAssembly.CodeBase.Substring(8, MyAssembly.CodeBase.LastIndexOf('/') - 7);

                System.Configuration.Configuration config = HandleConfiguration.DesEncriptarConfiguracion(AplicationPath + "ERDA.dll");

                ConnectionStringsSection connections = config.ConnectionStrings;

                SqlConnectionStringBuilder sqlConStrBuilder = new SqlConnectionStringBuilder { ConnectionString = connections.ConnectionStrings[NombreConexion].ConnectionString };
                return sqlConStrBuilder;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SQLAccessBuilder(int ConnectionTimeAwait)
{
		//MyBase.New()
		adDataAdapter = new SqlDataAdapter();
        adConnection = new SSQLConnection();
        ConfigdsConection(ConnectionTimeAwait);
    }

        public SQLAccessBuilder(string TableName)
	    {
		    //MyBase.New()
		    adDataAdapter = new SqlDataAdapter();
        adConnection = new SSQLConnection();
        ConfigdsConection();

        strTableName = TableName;
		    Mapping();

    }

        public SQLAccessBuilder(string TableName, DataColumn[] pk)
	        {
		        //MyBase.new()
		        adDataAdapter = new SqlDataAdapter();
        adConnection = new SSQLConnection();
        ConfigdsConection();

        strTableName = TableName;
		        pkColumns = pk;
		        dtMain = ObtenerEstructura(TableName);
        Mapping();
	        }

	    public SQLAccessBuilder(SqlTransaction trans, string TableName, DataColumn[] pk)
	    {
		    //MyBase.New(trans)
		    adDataAdapter = new SqlDataAdapter();
    adTransaction = trans;

		    strTableName = TableName;
		    pkColumns = pk;
		    dtMain = ObtenerEstructura(TableName);
    Mapping();
	    }

	    public SQLAccessBuilder(string session, string TableName)
	    {
		    adDataAdapter = new SqlDataAdapter();
    adConnection = new SSQLConnection();

    strTableName = TableName;
		    Mapping();
	    }

	    public SQLAccessBuilder(string session, string TableName, int ConnectionTimeWait)
	    {
		    //MyBase.New(sesion)
		    adDataAdapter = new SqlDataAdapter();
    adConnection = new SSQLConnection();
    strTableName = TableName;
		    Mapping();
	    }

	    public SQLAccessBuilder(string session, string TableName, DataColumn[] pk)
	    {
		    //MyBase.New(sesion)
		    adDataAdapter = new SqlDataAdapter();
    adConnection = new SSQLConnection();
    strTableName = TableName;
		    pkColumns = pk;

		    dtMain = ObtenerEstructura(TableName);
    Mapping();

	    }

	    public SQLAccessBuilder(SSQLConnection CConnecction)
	    {
		    //MyBase.New(sesion)
		    adDataAdapter = new SqlDataAdapter();
    adConnection = CConnecction;

	    }

        public SQLAccessBuilder(string session, SqlTransaction trans, string TableName, DataColumn[] pk)
 {
		adDataAdapter = new SqlDataAdapter();
        adTransaction = trans;

		strTableName = TableName;
		pkColumns = pk;

		dtMain = ObtenerEstructura(TableName);
        Mapping();
    }

        public SQLAccessBuilder(string session, SqlTransaction trans, string TableName)
	    {
		    //MyBase.New(sesion, trans)
		    adDataAdapter = new SqlDataAdapter();
        //MapearSesion(sesion)
        adTransaction = trans;

		    strTableName = TableName;
		    Mapping();

    }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region aCCIONES
        public void GetDataset(ref DataTable tabla)
        {
            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(pa_LOAD);
                adDataAdapter.Fill(tabla);
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                }
                adDataAdapter.Dispose();

            }
        }

        public void GetDataset(ref DataTable tabla, string procedimiento)
        {
            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(procedimiento);
                adDataAdapter.Fill(tabla);
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                }
                adDataAdapter.Dispose();
            }
        }

        public void GetDataset(ref DataTable tabla, string procedimiento, object[] parametros)
        {
            SqlDataAdapter tempVar = (SqlDataAdapter)adDataAdapter;
            try
            {
                tempVar.SelectCommand = GetLoadCommand(procedimiento);

                if (tempVar.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    tempVar.SelectCommand.Connection.Open();
                }


                if (adTransaction == null)
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder();
                    SqlCommandBuilder.DeriveParameters(tempVar.SelectCommand);
                }
                else
                {
                    SqlCommand parameter = (SqlCommand)tempVar.SelectCommand;
                    GetParameters(ref parameter, procedimiento);
                }

                int i = 0;
                for (i = 0; i <= parametros.GetUpperBound(0); i++)
                {
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Parameters[i + 1].Value = parametros[i];
                    }
                    else
                    {
                        tempVar.SelectCommand.Parameters[i].Value = parametros[i];
                    }
                }
                tempVar.Fill(tabla);



            }
            finally
            {
#if DEBUG
                if (tabla.HasErrors)
                {
                    foreach (DataRow dr in tabla.Rows)
                    {
                        if (dr.HasErrors)
                        {
                            string s = dr.RowError;
                        }
                    }
                }
#endif

                if (tempVar.SelectCommand != null)
                {
                    tempVar.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Connection.Close();
                    }
                }
                tempVar.Dispose();
            }
        }
        public void GetDataset(ref DataSet ds, string procedimiento, object[] parametros, string[] tablas)
        {
            SqlDataAdapter tempVar = (SqlDataAdapter)adDataAdapter;
            try
            {
                tempVar.SelectCommand = GetLoadCommand(procedimiento);

                if (tempVar.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    tempVar.SelectCommand.Connection.Open();
                }

                if (adTransaction == null)
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder();
                    SqlCommandBuilder.DeriveParameters(tempVar.SelectCommand);
                }
                else
                {
                    SqlCommand parameter = (SqlCommand)tempVar.SelectCommand;
                    GetParameters(ref parameter, procedimiento);
                }

                int i = 0;
                for (i = 0; i <= parametros.GetUpperBound(0); i++)
                {
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Parameters[i + 1].Value = parametros[i];
                    }
                    else
                    {
                        tempVar.SelectCommand.Parameters[i].Value = parametros[i];
                    }
                }
                i = 1;
                foreach (string tabla in tablas)
                {
                    tempVar.TableMappings.Add("Table" + i, tabla);
                    i += 1;
                }
                tempVar.Fill(ds);



            }
            finally
            {
#if DEBUG
                if (ds.HasErrors)
                {
                    foreach (string tb in tablas)
                    {
                        foreach (DataRow dr in ds.Tables[tb].Rows)
                        {
                            if (dr.HasErrors)
                            {
                                string s = dr.RowError;
                            }
                        }
                    }
                }
#endif

                if (tempVar.SelectCommand != null)
                {
                    tempVar.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        tempVar.SelectCommand.Connection.Close();
                    }
                }
                tempVar.Dispose();
            }
        }

        public void GetDataset(ref DataSet ds, string procedimiento, object[] parametros)
        {
            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(procedimiento);

                if (adDataAdapter.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    adDataAdapter.SelectCommand.Connection.Open();
                }

                if (adTransaction == null)
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder();
                    SqlCommandBuilder.DeriveParameters((SqlCommand)adDataAdapter.SelectCommand);
                }
                else
                {
                    SqlCommand parameter = (SqlCommand)adDataAdapter.SelectCommand;
                    GetParameters(ref parameter, procedimiento);
                }

                int i = 0;
                for (i = 0; i <= parametros.GetUpperBound(0); i++)
                {
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Parameters[i + 1].Value = parametros[i];
                    }
                    else
                    {
                        adDataAdapter.SelectCommand.Parameters[i].Value = parametros[i];
                    }
                }
                adDataAdapter.Fill(ds);
            }
            catch (Exception ss)
            {
                throw ss;
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Connection.Close();
                    }
                }
                adDataAdapter.Dispose();

            }
        }

        public void GetDataset<T>(ref T ds, string procedimiento, object[] parametros) where T : DataSet
        {
            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(procedimiento);

                if (adDataAdapter.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    adDataAdapter.SelectCommand.Connection.Open();
                }

                if (adTransaction == null)
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder();
                    SqlCommandBuilder.DeriveParameters((SqlCommand)adDataAdapter.SelectCommand);
                }
                else
                {
                    SqlCommand parameter = (SqlCommand)adDataAdapter.SelectCommand;
                    GetParameters(ref parameter, procedimiento);
                }

                int i = 0;
                for (i = 0; i <= parametros.GetUpperBound(0); i++)
                {
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Parameters[i + 1].Value = parametros[i];
                    }
                    else
                    {
                        adDataAdapter.SelectCommand.Parameters[i].Value = parametros[i];
                    }
                }
                adDataAdapter.Fill(ds);
            }
            catch (Exception ss)
            {
                throw ss;
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Connection.Close();
                    }
                }
                adDataAdapter.Dispose();

            }
        }

        public void GetDataset(ref DataSet ds, string procedimiento)
        {
            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(procedimiento);

                if (adDataAdapter.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    adDataAdapter.SelectCommand.Connection.Open();
                }
                adDataAdapter.Fill(ds);
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Connection.Close();
                    }
                }
                adDataAdapter.Dispose();
            }
        }

        public void GetDataset(ref DataSet ds, string procedimiento, object[] parametros, string Server, string BD)
        {

            string ServerActual = ((SSQLConnection)adConnection).Server;
            string BaseDatosActual = ((SSQLConnection)adConnection).DataBase;


            ((SSQLConnection)adConnection).Server = Server;
            ((SSQLConnection)adConnection).DataBase = BD;

            try
            {
                adDataAdapter.SelectCommand = GetLoadCommand(procedimiento);

                if (adDataAdapter.SelectCommand.Connection.State == ConnectionState.Closed)
                {
                    adDataAdapter.SelectCommand.Connection.Open();
                }

                if (adTransaction == null)
                {
                    SqlCommandBuilder cb = new SqlCommandBuilder();
                    SqlCommandBuilder.DeriveParameters((SqlCommand)adDataAdapter.SelectCommand);
                }
                else
                {
                    SqlCommand parameter = (SqlCommand)adDataAdapter.SelectCommand;
                    GetParameters(ref parameter, procedimiento);
                }

                int i = 0;
                for (i = 0; i <= parametros.GetUpperBound(0); i++)
                {
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Parameters[i + 1].Value = parametros[i];
                    }
                    else
                    {
                        adDataAdapter.SelectCommand.Parameters[i].Value = parametros[i];
                    }
                }
                adDataAdapter.Fill(ds);
            }
            finally
            {
                if (adDataAdapter.SelectCommand != null)
                {
                    adDataAdapter.SelectCommand.Dispose();
                    if (adTransaction == null)
                    {
                        adDataAdapter.SelectCommand.Connection.Close();
                    }
                }
                adDataAdapter.Dispose();
                ((SSQLConnection)adConnection).Server = ServerActual;
                ((SSQLConnection)adConnection).DataBase = BaseDatosActual;
            }

        }

        public DataSet Insert(DataSet dsInsert)
        {
            DataSet tempInsert = null;
            DataRow[] drsACambiar = null;
            try
            {
                if (dsInsert != null)
                {
                    adDataAdapter.InsertCommand = GetInsertCommand();
                    drsACambiar = dsInsert.Tables[strTableName].Select("", "", DataViewRowState.Added);
                    adDataAdapter.Update(drsACambiar);
                }
            }
            finally
            {
                tempInsert = dsInsert;
            }
            return tempInsert;
        }

        public DataSet Update(DataSet dsUpdate)
        {
            DataSet tempUpdate = null;
            DataRow[] drsACambiar = null;
            try
            {
                if (dsUpdate != null)
                {
                    adDataAdapter.UpdateCommand = GetUpdatecommand();
                    drsACambiar = dsUpdate.Tables[strTableName].Select("", "", DataViewRowState.ModifiedCurrent);
                    adDataAdapter.Update(drsACambiar);
                }
            }
            finally
            {
                tempUpdate = dsUpdate;
            }
            return tempUpdate;
        }

        public DataSet Delete(DataSet dsDelete)
        {
            DataSet tempDelete = null;
            try
            {
                if (dsDelete != null)
                {
                    adDataAdapter.DeleteCommand = GetDeleteCommand();
                    adDataAdapter.Update(ObtenerFilasEliminadas(dsDelete.Tables[strTableName]));
                }
            }
            finally
            {
                tempDelete = dsDelete;
            }
            return tempDelete;
        }

        public static SSQLConnection GetClassSSQLConnection()
        {
            SSQLConnection sql = new SSQLConnection();
            SqlConnectionStringBuilder builder = AppConectionString(USER);
            sql.Server = builder.DataSource;
            sql.DataBase = builder.InitialCatalog;
            sql.UUSerrr = builder.UserID;
            sql.Passwordd = builder.Password;
            sql.CConnectionString = builder.ConnectionString;
            return sql;
        }
        #endregion
    }
}
