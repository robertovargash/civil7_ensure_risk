using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using ERDA;
using DataMapping.Data;

namespace EnsureRiskWS
{
    /// <summary>
    /// Summary description for WebServiceRisk
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServiceRisk : System.Web.Services.WebService
    {
        [WebMethod]
        public DataSet GetRiskTreeString(object[] param)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Diagram.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectRiskTreeFilterSTRING", param);
                ds.Tables[1].TableName = DT_Diagram_Damages.TABLE_NAME;
                ds.Tables[2].TableName = DT_Risk.TABLE_NAME;
                ds.Tables[3].TableName = DT_RiskStructure.TABLE_NAME;
                ds.Tables[4].TableName = DT_Risk_Damages.TABLE_NAME;
                ds.Tables[5].TableName = DT_RISK_WBS.TABLE_NAME;
                ds.Tables[6].TableName = DT_WBS_RISK_DAMAGE.TABLE_NAME;
                ds.Tables[7].TableName = DT_Role_Risk.TABLENAME;
                ds.Tables[8].TableName = DT_CounterM.TABLE_NAME;
                ds.Tables[9].TableName = DT_CounterM_Damage.TABLE_NAME;
                ds.Tables[10].TableName = DT_CM_WBS.TABLE_NAME;
                ds.Tables[11].TableName = DT_WBS_CM_Damage.TABLE_NAME;
                ds.Tables[12].TableName = DT_Role_CM.TABLENAME;
                ds.Tables[13].TableName = DT_User_Role.TABLE_NAME;
                ds.Tables[14].TableName = DT_Role.ROLE_TABLE;
                ds.Tables[15].TableName = DT_DefaulRisk.Risk_TABLA;
                ds.Tables[16].TableName = DT_Groupe.TABLE_NAME;
                //ds.Tables[14].TableName = DT_User_WBS.TABLE_NAME;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet GetDefaultRisk()
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_DefaulRisk.Risk_TABLA);
                SQL.GetDataset(ref ds, "pa_SelectDefaultRisk");
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet SaveRisk(DataSet ds)
        {
            SqlConnection sql = new SqlConnection();
            SqlTransaction trans;
            SSQLConnection conection = SQLAccessBuilder.GetClassSSQLConnection();
            trans = (SqlTransaction)conection.BeginTransaction();
            try
            {

                SQLAccessBuilder trDA = new SQLAccessBuilder(trans, ds.Tables[DT_Damage.TABLE_NAME].TableName, ds.Tables[DT_Damage.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder riskTreeDA = new SQLAccessBuilder(trans, ds.Tables[DT_Diagram.TABLE_NAME].TableName, ds.Tables[DT_Diagram.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder riskTreeTopRiskDA = new SQLAccessBuilder(trans, ds.Tables[DT_Diagram_Damages.TABLE_NAME].TableName, ds.Tables[DT_Diagram_Damages.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder risk_TopDA = new SQLAccessBuilder(trans, ds.Tables[DT_Risk_Damages.TABLE_NAME].TableName, ds.Tables[DT_Risk_Damages.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder risk_WBSDA = new SQLAccessBuilder(trans, ds.Tables[DT_RISK_WBS.TABLE_NAME].TableName, ds.Tables[DT_RISK_WBS.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder counterMDA = new SQLAccessBuilder(trans, ds.Tables[DT_CounterM.TABLE_NAME].TableName, ds.Tables[DT_CounterM.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder riskDA = new SQLAccessBuilder(trans, ds.Tables[DT_Risk.TABLE_NAME].TableName, ds.Tables[DT_Risk.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder riskStructDA = new SQLAccessBuilder(trans, ds.Tables[DT_RiskStructure.TABLE_NAME].TableName, ds.Tables[DT_RiskStructure.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder counterm_toprDA = new SQLAccessBuilder(trans, ds.Tables[DT_CounterM_Damage.TABLE_NAME].TableName, ds.Tables[DT_CounterM_Damage.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder counterm_WBSDA = new SQLAccessBuilder(trans, ds.Tables[DT_CM_WBS.TABLE_NAME].TableName, ds.Tables[DT_CM_WBS.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder roleriskDA = new SQLAccessBuilder(trans, ds.Tables[DT_Role_Risk.TABLENAME].TableName, ds.Tables[DT_Role_Risk.TABLENAME].PrimaryKey);
                SQLAccessBuilder counterm_roleDA = new SQLAccessBuilder(trans, ds.Tables[DT_Role_CM.TABLENAME].TableName, ds.Tables[DT_Role_CM.TABLENAME].PrimaryKey);
                SQLAccessBuilder defaultRiskDA = new SQLAccessBuilder(trans, ds.Tables[DT_DefaulRisk.Risk_TABLA].TableName, ds.Tables[DT_DefaulRisk.Risk_TABLA].PrimaryKey);
                SQLAccessBuilder groupeDA = new SQLAccessBuilder(trans, ds.Tables[DT_Groupe.TABLE_NAME].TableName, ds.Tables[DT_Groupe.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder wbsriskdamageDA = new SQLAccessBuilder(trans, ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].TableName, ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].PrimaryKey);
                SQLAccessBuilder wbscmdamageDA = new SQLAccessBuilder(trans, ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].TableName, ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].PrimaryKey);

                groupeDA.Delete(ds);
                groupeDA.Update(ds);
                groupeDA.Insert(ds);

                trDA.Delete(ds);
                riskStructDA.Delete(ds);
                roleriskDA.Delete(ds);
                wbsriskdamageDA.Delete(ds);
                risk_WBSDA.Delete(ds);
                risk_TopDA.Delete(ds);
                riskDA.Delete(ds);
                counterm_roleDA.Delete(ds);
                wbscmdamageDA.Delete(ds);
                counterm_WBSDA.Delete(ds);
                counterm_toprDA.Delete(ds);
                counterMDA.Delete(ds);
                riskTreeTopRiskDA.Delete(ds);
                riskTreeDA.Delete(ds);
                defaultRiskDA.Delete(ds);

                trDA.Update(ds);
                riskStructDA.Update(ds);
                roleriskDA.Update(ds);
                wbsriskdamageDA.Update(ds);
                risk_WBSDA.Update(ds);
                risk_TopDA.Update(ds);
                riskDA.Update(ds);
                wbscmdamageDA.Update(ds);
                counterm_WBSDA.Update(ds);
                counterm_toprDA.Update(ds);
                counterMDA.Update(ds);
                counterm_roleDA.Update(ds);
                riskTreeTopRiskDA.Update(ds);
                riskTreeDA.Update(ds);
                defaultRiskDA.Update(ds);

                trDA.Insert(ds);
                riskTreeDA.Insert(ds);
                riskTreeTopRiskDA.Insert(ds);
                riskDA.Insert(ds);
                riskStructDA.Insert(ds);
                risk_TopDA.Insert(ds);
                risk_WBSDA.Insert(ds);
                wbsriskdamageDA.Insert(ds);
                roleriskDA.Insert(ds);
                counterMDA.Insert(ds);
                counterm_toprDA.Insert(ds);
                counterm_WBSDA.Insert(ds);
                wbscmdamageDA.Insert(ds);
                counterm_roleDA.Insert(ds);
                defaultRiskDA.Insert(ds);

                //riskDA.Update(ds);

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

        [WebMethod]
        public DataSet GetRiskTreeID(object[] param)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(DT_Diagram.TABLE_NAME);
                SQL.GetDataset(ref ds, "pa_SelectRiskTreeFilterByID", param);
                ds.Tables[1].TableName = DT_Diagram_Damages.TABLE_NAME;
                ds.Tables[2].TableName = DT_Risk.TABLE_NAME;
                ds.Tables[3].TableName = DT_RiskStructure.TABLE_NAME;
                ds.Tables[4].TableName = DT_Risk_Damages.TABLE_NAME;                
                ds.Tables[5].TableName = DT_RISK_WBS.TABLE_NAME;
                ds.Tables[6].TableName = DT_WBS_RISK_DAMAGE.TABLE_NAME;
                ds.Tables[7].TableName = DT_Role_Risk.TABLENAME;
                ds.Tables[8].TableName = DT_CounterM.TABLE_NAME;
                ds.Tables[9].TableName = DT_CounterM_Damage.TABLE_NAME;
                ds.Tables[10].TableName = DT_CM_WBS.TABLE_NAME;
                ds.Tables[11].TableName = DT_WBS_CM_Damage.TABLE_NAME;
                ds.Tables[12].TableName = DT_Role_CM.TABLENAME;
                ds.Tables[13].TableName = DT_User_Role.TABLE_NAME;
                ds.Tables[14].TableName = DT_Role.ROLE_TABLE;
                ds.Tables[15].TableName = DT_DefaulRisk.Risk_TABLA;
                ds.Tables[16].TableName = DT_Groupe.TABLE_NAME;
                //ds.Tables[14].TableName = DT_User_WBS.TABLE_NAME;
                userds.Merge(ds);
                return userds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public DataSet GetElementData(string sp, string tablename)
        {
            try
            {
                UserDataSet userds = new UserDataSet();
                DataSet ds = new DataSet();
                SQLAccessBuilder SQL = new SQLAccessBuilder(tablename);
                SQL.GetDataset(ref ds, sp);
                ds.Tables[0].TableName = tablename;
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
