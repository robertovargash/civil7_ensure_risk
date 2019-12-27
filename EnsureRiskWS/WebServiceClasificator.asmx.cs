using DataMapping.Data;
using ERDA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceClasificator
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceClasificator : System.Web.Services.WebService
    {
        [WebMethod]
        public DataSet GetAllClasifications()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Clasificator.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectClasificator");
                ds.Tables[0].TableName = DT_Clasificator.TABLE_NAME;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
