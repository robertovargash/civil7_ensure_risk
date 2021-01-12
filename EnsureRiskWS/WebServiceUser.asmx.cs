using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using DataMapping.Data;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceUser
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceUser : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetUserData()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                using (SQLAccessBuilder SQL = new SQLAccessBuilder(DT_User.TABLE_NAME))
                {
                    SQL.GetDataset(ref ds, "pa_SelectUsername");
                    ds.Tables[0].TableName = DT_User.TABLE_NAME;
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
        public DataSet GetUserRolesData(object[] param)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                using (SQLAccessBuilder SQL = new SQLAccessBuilder(DT_User.TABLE_NAME))
                {
                    SQL.GetDataset(ref ds, "pa_SelectUserWithRole", param);
                    ds.Tables[1].TableName = DT_User_Role.TABLE_NAME;
                    //ds.Tables[2].TableName = DT_User_WBS.TABLE_NAME;
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
        public DataSet SaveUser(DataSet ds)
        {
            SqlConnection sql = new SqlConnection();

            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                SQLAccessBuilder userDA = new SQLAccessBuilder(trans, ds.Tables[DT_User.TABLE_NAME].TableName, ds.Tables[DT_User.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder userRoleDA = new SQLAccessBuilder(trans, ds.Tables[DT_User_Role.TABLE_NAME].TableName, ds.Tables[DT_User_Role.TABLE_NAME].PrimaryKey);
                //SQLAccessBuilder userWBSDA = new SQLAccessBuilder(trans, ds.Tables[DT_User_WBS.TABLE_NAME].TableName, ds.Tables[DT_User_WBS.TABLE_NAME].PrimaryKey);

                userDA.Delete(ds);
                userRoleDA.Delete(ds);
                //userWBSDA.Delete(ds);

                userDA.Update(ds);

                userDA.Insert(ds);
                userRoleDA.Insert(ds);
                //userWBSDA.Insert(ds);

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
