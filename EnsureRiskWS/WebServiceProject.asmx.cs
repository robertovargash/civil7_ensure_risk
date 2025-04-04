﻿using System;
using System.Web.Services;
using DataMapping.Data;
using System.Data;
using System.Data.SqlClient;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceProject
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceProject : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetAllProjects()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                using (SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Project.TABLE_NAME))
                {
                    SQL.GetDataset(ref ds, "pa_SelectProjectFiltered", new object[] { -9999 });
                    ds.Tables[0].TableName = DT_Project.TABLE_NAME;
                    ds.Tables[1].TableName = DT_WBS.TABLE_NAME;
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
        public DataSet SaveProject(DataSet ds)
        {
            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                SQLAccessBuilder trProject = new SQLAccessBuilder(trans, ds.Tables[DT_Project.TABLE_NAME].TableName, ds.Tables[DT_Project.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder trWBS = new SQLAccessBuilder(trans, ds.Tables[DT_WBS.TABLE_NAME].TableName, ds.Tables[DT_WBS.TABLE_NAME].PrimaryKey);

                trProject.Delete(ds);
                trWBS.Delete(ds);

                trProject.Update(ds);
                trWBS.Update(ds);

                trProject.Insert(ds);
                trWBS.Insert(ds);

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
