using DataMapping.Data;
using ERDA;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EnsureRiskWS
{
    public class AuthenticationController
    {
        private string cadenaError;

        public AuthenticationController()
        {

        }

        public string ValidateUser(string usuario, byte[] contrasena, string ip, string usuarioPC, DataSet dsConfiguracion)
        {
            string tempValidarUsuario = null;
            string sesion = null;

            DataSet dsTemp = new DataSet();
            DataRow rowUsuario = null;
            SQLAccessBuilder adUsuario = new SQLAccessBuilder(DT_User.User_TABLA);
            //Dim adSesiones As New AD.SQLAccesoDatosUnica(SesionesDatos.SESIONES_TABLA)

            tempValidarUsuario = string.Empty;
            try
            {
                string[] tables = { DT_User.User_TABLA, DT_Role.ROLE_TABLE };
                adUsuario.GetDataset(ref dsTemp, "pa_ValidateFullUser", new object[] { usuario, contrasena });
                dsTemp.Tables[1].TableName = DT_Role.ROLE_TABLE;
                dsTemp.Tables[2].TableName = DT_User_Operation.TABLE_NAME;
                dsConfiguracion.Merge(dsTemp);
                DataTable dtUsuarios = dsConfiguracion.Tables[DT_User.User_TABLA];
                if (dtUsuarios.Rows.Count == 0)
                {
                    return "";
                }
                else
                {
                    rowUsuario = dtUsuarios.Rows[0];
                    sesion = ObtenerGUID();
                    DateTime fecha = DateTime.Now;
                    return sesion;
                }
            }
            catch (Exception excep)
            {
                cadenaError = excep.Message;
                tempValidarUsuario = "";
            }
            return tempValidarUsuario;
        }

        private static string CrearGUID(byte longitud)
        {
            int contador = 0;
            string GUID = "";
            const string VALORES = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (contador = 1; contador <= longitud; contador++)
            {
                GUID = GUID + VALORES.Substring((int)(VBMath.Rnd(1) * VALORES.Length), 1);
            }
            return GUID;
        }

        public static string ObtenerGUID()
        {
            return CrearFechaGUID() + "-" + CrearGUID(8) + "-" + CrearGUID(4);
        }

        private static string CrearFechaGUID()
        {
            return (DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second).ToString();
        }
    }
}