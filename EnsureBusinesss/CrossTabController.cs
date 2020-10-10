using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureBusinesss
{
    public class CrossTabController
    {
        public static void AjustarProbabilidadRisk(DataRow drRiskDamage, DataTable dtRisk_WBS, decimal IdWBSFilter)
        {
            try
            {
                if (dtRisk_WBS.Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_RISK_WBS.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK]).Any())
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = dtRisk_WBS.Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_RISK_WBS.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK]).First()[DT_RISK_WBS.PROBABILITY];
                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.PROBABILITY] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void AjustarProbabilidadCM(DataRow drRiskDamage, DataTable dtCM_WBS, decimal IdWBSFilter)
        {
            try
            {
                if (dtCM_WBS.Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_CM_WBS.ID_CM + " = " + drRiskDamage[DT_CounterM_Damage.ID_COUNTERM]).Any())
                {
                    drRiskDamage[DT_CounterM_Damage.RISK_REDUCTION] = dtCM_WBS.Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_CM_WBS.ID_CM + " = " + drRiskDamage[DT_CounterM_Damage.ID_COUNTERM]).First()[DT_CM_WBS.PROBABILITY];
                }
            }
            catch
            {
                throw new Exception("Error ajustando ProbCM");
            }
        }

        public static void AjustarDamagesRisk(DataRow drRiskDamage, DataTable dtRisk_WBS_Damage, decimal IdWBSFilter)
        {
            try
            {
                if (dtRisk_WBS_Damage.Select(DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK] + " and " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + drRiskDamage[DT_Risk_Damages.ID_DAMAGE]).Any())
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = dtRisk_WBS_Damage.Select(DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK] + " and " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + drRiskDamage[DT_Risk_Damages.ID_DAMAGE]).First()[DT_WBS_RISK_DAMAGE.VALUE];
                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.VALUE] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = DBNull.Value;
                }
            }
            catch
            {
                throw new Exception("Error ajustandoDamagesRisk");
            }
        }

        public static void AjustarDamagesCM(DataRow drCMDamage, DataTable dtCM_WBS_Damage, decimal IdWBSFilter)
        {
            try
            {
                if (dtCM_WBS_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_CM_Damage.ID_CM + " = " + drCMDamage[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + drCMDamage[DT_CounterM_Damage.ID_DAMAGE]).Any())
                {
                    drCMDamage[DT_CounterM_Damage.VALUE] = dtCM_WBS_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_CM_Damage.ID_CM + " = " + drCMDamage[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + drCMDamage[DT_CounterM_Damage.ID_DAMAGE]).First()[DT_WBS_CM_Damage.VALUE];
                }
            }
            catch
            {
                throw new Exception("Error ajustando DmaagesCM");
            }
        }

        public static void ColumnasTextoRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Risk")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasNumerosRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Risk" && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" && item.Header.ToString() != "Father" && item.Header.ToString() != "Active?")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasTextoCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Counter M.")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ColumnasNumerosCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Counter M." && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" && item.Header.ToString() != "Risk" && item.Header.ToString() != "Status")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void CleanDynamicCMColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Risk Reduction(%)" && dataGrid.Columns[i].Header.ToString() != "Counter M." && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }

        public static void CleanDynamicRiskColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Probability (%)" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Father" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }
    }
}
