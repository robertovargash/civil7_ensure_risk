using System;
using System.Web.Services;
using DataMapping.Data;
using System.Data;
using System.Data.SqlClient;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceMyLog
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceMyLog : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetAllLogs(object[] param)
        {
            try
            {
                MyLogDataset logDS = new MyLogDataset();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_MyLog.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectMyLogFiltered", param);
                ds.Tables[0].TableName = DT_MyLog.TABLE_NAME;
                logDS.Merge(ds);
                return logDS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet InsertLog(DataSet ds)
        {
            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                using (SQLAccessBuilder trDA = new SQLAccessBuilder(trans, ds.Tables[DT_MyLog.TABLE_NAME].TableName, ds.Tables[DT_MyLog.TABLE_NAME].PrimaryKey))
                {
                    trDA.Insert(ds);
                }
                if (ds.HasErrors)
                {
                    conection.AbortTransaction(trans);
                }
                else
                {
                    conection.EndTransaction(trans);
                }
                return ds;
            }
            catch (Exception ex)
            {
                conection.AbortTransaction(trans);
                throw ex;
            }
        }
    }
}
