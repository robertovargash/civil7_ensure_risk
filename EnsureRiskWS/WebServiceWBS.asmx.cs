using System;
using System.Web.Services;
using DataMapping.Data;
using System.Data;
using System.Data.SqlClient;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceWBS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceWBS : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetAllWBSFiltered(object[] param)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();                
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_WBS.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectWBSFiltered", param);
                ds.Tables[0].TableName = DT_WBS.TABLE_NAME;
                ds.Tables[1].TableName = DT_WBS_STRUCTURE.TABLE_NAME;
                userds.Merge(ds);
                SQL.Dispose();
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet GetAllWBS()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_WBS.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectWBS");
                ds.Tables[0].TableName = DT_WBS.TABLE_NAME;
                ds.Tables[1].TableName = DT_WBS_STRUCTURE.TABLE_NAME;
                userds.Merge(ds);
                SQL.Dispose();
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet SaveWBS(DataSet ds)
        {
            //SqlConnection sql = new SqlConnection();
            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                SQLAccessBuilder trDA = new SQLAccessBuilder(trans, ds.Tables[DT_WBS.TABLE_NAME].TableName, ds.Tables[DT_WBS.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder StrDA = new SQLAccessBuilder(trans, ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].TableName, ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].PrimaryKey);

                StrDA.Delete(ds);
                trDA.Delete(ds);

                StrDA.Update(ds);
                trDA.Update(ds);

                trDA.Insert(ds);
                StrDA.Insert(ds);

                if (ds.HasErrors)
                {
                    conection.AbortTransaction(trans);
                }
                else
                {
                    conection.EndTransaction(trans);
                }
                trDA.Dispose();
                StrDA.Dispose();
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
