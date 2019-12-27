using System;
using System.Web.Services;
using DataMapping.Data;
using System.Data;
using System.Data.SqlClient;
using ERDA;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceGroupe
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceGroupe : System.Web.Services.WebService
    {

        [WebMethod]
        public DataSet GetAllGroupes()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Project.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectGroupe");
                ds.Tables[0].TableName = DT_Groupe.TABLE_NAME;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet SaveGroupe(DataSet ds)
        {
            SqlConnection sql = new SqlConnection();

            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            //conection = (SqlConnection)
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {


                SQLAccessBuilder trDA = new SQLAccessBuilder(trans, ds.Tables[DT_Groupe.TABLE_NAME].TableName, ds.Tables[DT_Groupe.TABLE_NAME].PrimaryKey);

                trDA.Delete(ds);

                trDA.Update(ds);

                trDA.Insert(ds);

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
