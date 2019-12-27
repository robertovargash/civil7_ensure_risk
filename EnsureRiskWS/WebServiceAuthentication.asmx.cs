using System;
using System.Web.Services;
using System.Data;
using DataMapping.Data;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceAuthentication
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceAuthentication : System.Web.Services.WebService
    {

        [WebMethod()]
        public DataSet AuthenticateUsers(string usser, byte[] password, ref string MensajeError, string _usuarioPC, string _pcNombreIP, ref string Sesion)
        {
            DataSet tempAutenticarUsuarios = new UserDataSet();
            try
            {
                Sesion = (new AuthenticationController()).ValidateUser(usser, password, _usuarioPC, _pcNombreIP, tempAutenticarUsuarios);
            }
            catch (Exception ex)
            {
                MensajeError = ex.Message;
            }
            return tempAutenticarUsuarios;
        }

    }
}
