﻿using System;
using System.Web.Services;
using DataMapping.Data;
using System.Data;
using System.Data.SqlClient;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceTopRisk
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceTopRisk : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetAllTopRisk()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                using (SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Damage.TABLE_NAME))
                {
                    SQL.GetDataset(ref ds, "pa_SelectTopRisk");
                    ds.Tables[0].TableName = DT_Damage.TABLE_NAME;
                    userds.Merge(ds);
                    return userds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet SaveTopRisk(DataSet ds)
        {
            SqlConnection sql = new SqlConnection();

            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                using (SQLAccessBuilder trDA = new SQLAccessBuilder(trans, ds.Tables[DT_Damage.TABLE_NAME].TableName, ds.Tables[DT_Damage.TABLE_NAME].PrimaryKey))
                {
                    trDA.Delete(ds);

                    trDA.Update(ds);

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
