using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using DataMapping.Data;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceRole
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceRole : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetRolesData()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Role.ROLE_TABLE);
                SQL.GetDataset(ref ds, "pa_SelectRole");
                ds.Tables[0].TableName = DT_Role.ROLE_TABLE;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [WebMethod]
        public DataSet GetRolesOperation(object[] param)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Role.ROLE_TABLE);
                SQL.GetDataset(ref ds, "pa_SelectRoleWithOperation", param);
                ds.Tables[1].TableName = DT_OperationRole.OPERATION_ROLE_TABLE;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet GetOperation()
        {
            try
            {
                UserDataSet user = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Operation.OPERATION_TABLE);
                SQL.GetDataset(ref ds, "pa_SelectOperation");
                user.Merge(ds);
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet SaveRole(DataSet ds)
        {
            SqlConnection sql = new SqlConnection();

            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {
                SQLAccessBuilder roleDA = new SQLAccessBuilder(trans, ds.Tables[DT_Role.ROLE_TABLE].TableName, ds.Tables[DT_Role.ROLE_TABLE].PrimaryKey);
                SQLAccessBuilder opDA = new SQLAccessBuilder(trans, ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].TableName, ds.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].PrimaryKey);

                roleDA.Delete(ds);
                opDA.Delete(ds);

                roleDA.Update(ds);
                opDA.Update(ds);

                roleDA.Insert(ds);
                opDA.Insert(ds);

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
